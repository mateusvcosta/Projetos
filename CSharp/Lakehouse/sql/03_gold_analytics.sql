CREATE SCHEMA IF NOT EXISTS tunaengine.gold;

CREATE OR REPLACE TABLE tunaengine.gold.dim_member AS
SELECT
    Id_Member AS member_id,
    Name AS member_name,
    Tuna_Name AS tuna_name,
    Email AS email,
    Id_Role AS role_id,
    Voice_Type AS voice_type,
    Instrument AS instrument,
    Date_added AS added_at
FROM tunaengine.silver.members;

CREATE OR REPLACE TABLE tunaengine.gold.dim_inventory AS
SELECT
    Id_inventory AS inventory_id,
    Designation AS designation,
    Instrument_Type AS instrument_type,
    Price AS price,
    Condition AS item_condition,
    Brand AS brand,
    Date_added AS added_at
FROM tunaengine.silver.inventory;

CREATE OR REPLACE TABLE tunaengine.gold.dim_event AS
SELECT
    Id_Event AS event_id,
    Name AS event_name,
    Date_Hour AS event_at,
    Location AS location
FROM tunaengine.silver.events;

CREATE OR REPLACE TABLE tunaengine.gold.fact_event_participation AS
SELECT
    Id_Event AS event_id,
    Id_Member AS member_id,
    Attendance_Status AS attendance_status
FROM tunaengine.silver.event_participants;

CREATE OR REPLACE TABLE tunaengine.gold.fact_event_inventory AS
SELECT
    Id_Event AS event_id,
    Id_Inventory AS inventory_id,
    Designation AS designation,
    Quantity AS quantity
FROM tunaengine.silver.event_inventory;

CREATE OR REPLACE TABLE tunaengine.gold.fact_event_ride AS
SELECT
    Id_Event AS event_id,
    Id_Driver AS driver_id,
    Pickup_Location AS pickup_location,
    Departure_Hour AS departure_hour,
    Available_Seats AS available_seats,
    Passenger_Count AS passenger_count,
    Available_Seats - Passenger_Count AS remaining_seats
FROM tunaengine.silver.event_rides;

CREATE OR REPLACE TABLE tunaengine.gold.kpi_event_attendance AS
SELECT
    event_id,
    COUNT(*) AS invited_members,
    SUM(CASE
        WHEN lower(attendance_status) IN ('present', 'confirmed', 'attending', 'presente')
        THEN 1 ELSE 0
    END) AS attending_members,
    ROUND(
        100.0 * SUM(CASE
            WHEN lower(attendance_status) IN ('present', 'confirmed', 'attending', 'presente')
            THEN 1 ELSE 0
        END) / NULLIF(COUNT(*), 0),
        2
    ) AS attendance_rate_percent
FROM tunaengine.gold.fact_event_participation
GROUP BY event_id;

CREATE OR REPLACE TABLE tunaengine.gold.kpi_event_overview AS
SELECT
    e.event_id,
    e.event_name,
    e.event_at,
    date(e.event_at) AS event_date,
    year(e.event_at) AS event_year,
    month(e.event_at) AS event_month,
    e.location,
    COALESCE(p.invited_members, 0) AS invited_members,
    COALESCE(p.attending_members, 0) AS attending_members,
    COALESCE(p.attendance_rate_percent, 0) AS attendance_rate_percent,
    COALESCE(i.instrument_types, 0) AS instrument_types,
    COALESCE(i.total_instruments, 0) AS total_instruments,
    COALESCE(r.ride_count, 0) AS ride_count,
    COALESCE(r.passenger_capacity, 0) AS passenger_capacity,
    COALESCE(r.passenger_count, 0) AS passenger_count
FROM tunaengine.gold.dim_event e
LEFT JOIN tunaengine.gold.kpi_event_attendance p ON p.event_id = e.event_id
LEFT JOIN (
    SELECT
        event_id,
        COUNT(DISTINCT inventory_id) AS instrument_types,
        SUM(quantity) AS total_instruments
    FROM tunaengine.gold.fact_event_inventory
    GROUP BY event_id
) i ON i.event_id = e.event_id
LEFT JOIN (
    SELECT
        event_id,
        COUNT(*) AS ride_count,
        SUM(available_seats) AS passenger_capacity,
        SUM(passenger_count) AS passenger_count
    FROM tunaengine.gold.fact_event_ride
    GROUP BY event_id
) r ON r.event_id = e.event_id;

CREATE OR REPLACE TABLE tunaengine.gold.kpi_event_monthly AS
SELECT
    event_year,
    event_month,
    location,
    COUNT(*) AS event_count,
    ROUND(AVG(attendance_rate_percent), 2) AS average_attendance_rate_percent,
    SUM(attending_members) AS attending_members,
    SUM(total_instruments) AS instruments_requested,
    SUM(passenger_count) AS passengers
FROM tunaengine.gold.kpi_event_overview
GROUP BY event_year, event_month, location;

CREATE OR REPLACE TABLE tunaengine.gold.kpi_inventory_usage AS
SELECT
    inventory_id,
    designation,
    COUNT(DISTINCT event_id) AS events_using_item,
    SUM(quantity) AS total_quantity_requested
FROM tunaengine.gold.fact_event_inventory
GROUP BY inventory_id, designation;
