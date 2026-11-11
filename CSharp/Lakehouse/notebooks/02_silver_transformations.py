# Databricks notebook source
# MAGIC %md
# MAGIC # TunaEngine - Silver
# MAGIC
# MAGIC Normalização e validação dos dados ingeridos na camada Bronze.

# COMMAND ----------

catalog = dbutils.widgets.text("catalog", "tunaengine")
catalog = dbutils.widgets.get("catalog")
if not catalog:
    raise ValueError("O catálogo é obrigatório.")

spark.sql(f"CREATE SCHEMA IF NOT EXISTS `{catalog}`.silver")

def latest_records(table_name: str, key_columns: list[str]):
    columns = ", ".join(f"`{column}`" for column in key_columns)
    return spark.sql(f"""
        SELECT * EXCEPT (_source_database, _source_collection)
        FROM (
            SELECT *,
                   row_number() OVER (
                       PARTITION BY {columns}
                       ORDER BY _ingested_at DESC
                   ) AS _rn
            FROM `{catalog}`.bronze.{table_name}
        )
        WHERE _rn = 1
    """).drop("_rn")

def save(table_name: str, dataframe):
    (
        dataframe.write
        .format("delta")
        .mode("overwrite")
        .option("overwriteSchema", "true")
        .saveAsTable(f"`{catalog}`.silver.{table_name}")
    )

# COMMAND ----------

from pyspark.sql.functions import col, trim, to_timestamp, explode, size

members = latest_records("members", ["Id_Member"])
members = (
    members
    .filter(col("Id_Member").isNotNull())
    .withColumn("Name", trim(col("Name")))
    .withColumn("Email", trim(col("Email")))
    .withColumn("Date_added", to_timestamp("Date_added"))
    .drop("Password", "PasswordHash")
)
save("members", members)

inventory = latest_records("inventory", ["Id_inventory"])
inventory = (
    inventory
    .filter(col("Id_inventory").isNotNull())
    .withColumn("Designation", trim(col("Designation")))
    .withColumn("Date_added", to_timestamp("Date_added"))
)
save("inventory", inventory)

events = latest_records("events", ["Id_Event"])
events = (
    events
    .filter(col("Id_Event").isNotNull())
    .withColumn("Date_Hour", to_timestamp("Date_Hour"))
    .withColumn("Name", trim(col("Name")))
)
save("events", events)

event_participants = (
    events
    .select("Id_Event", explode("Participants").alias("participant"))
    .select(
        "Id_Event",
        col("participant.Id_Member").alias("Id_Member"),
        col("participant.Attendance_Status").alias("Attendance_Status"),
    )
    .filter(col("Id_Member").isNotNull())
)
save("event_participants", event_participants)

event_inventory = (
    events
    .select("Id_Event", explode("Instruments_Needed").alias("instrument"))
    .select(
        "Id_Event",
        col("instrument.Id_Inventory").alias("Id_Inventory"),
        col("instrument.Designation").alias("Designation"),
        col("instrument.Quantity").cast("int").alias("Quantity"),
    )
    .filter(col("Id_Inventory").isNotNull())
)
save("event_inventory", event_inventory)

event_rides = (
    events
    .select("Id_Event", explode("Boleias").alias("ride"))
    .select(
        "Id_Event",
        col("ride.Id_Driver").alias("Id_Driver"),
        col("ride.Pickup_Location").alias("Pickup_Location"),
        col("ride.Departure_Hour").alias("Departure_Hour"),
        col("ride.Available_Seats").cast("int").alias("Available_Seats"),
        col("ride.Passenger_Ids").alias("Passenger_Ids"),
    )
    .withColumn("Passenger_Count", size("Passenger_Ids"))
    .filter(col("Id_Driver").isNotNull())
)
save("event_rides", event_rides)

documents = latest_records("documents", ["Id_Documento"])
documents = documents.withColumn("Date_Created", to_timestamp("Date_Created"))
save("documents", documents)
