# Databricks notebook source
# MAGIC %md
# MAGIC # TunaEngine - Bronze
# MAGIC
# MAGIC Ingestão das coleções operacionais MongoDB para tabelas Delta brutas.

# COMMAND ----------

from pyspark.sql.functions import current_timestamp, lit

catalog = dbutils.widgets.text("catalog", "tunaengine")
secret_scope = dbutils.widgets.text("secret_scope", "tunaengine")
secret_key = dbutils.widgets.text("secret_key", "connection-uri")

catalog = dbutils.widgets.get("catalog")
secret_scope = dbutils.widgets.get("secret_scope")
secret_key = dbutils.widgets.get("secret_key")

if not catalog or not secret_scope or not secret_key:
    raise ValueError("catalog, secret_scope e secret_key são obrigatórios.")

connection_uri = dbutils.secrets.get(secret_scope, secret_key)
if not connection_uri:
    raise ValueError("A URI MongoDB obtida do Secret Scope está vazia.")

spark.sql(f"CREATE CATALOG IF NOT EXISTS `{catalog}`")
spark.sql(f"CREATE SCHEMA IF NOT EXISTS `{catalog}`.bronze")

# Os nomes correspondem aos valores por defeito dos serviços TunaEngine.
sources = [
    ("members", "Members", "Members"),
    ("roles", "Members", "Roles"),
    ("events", "Events", "Events"),
    ("inventory", "Inventory", "Inventory"),
    ("documents", "Documents", "Documents"),
    ("document_types", "Doc_Type", "Doc_Type"),
]

# COMMAND ----------

for table_name, database_name, collection_name in sources:
    source_df = (
        spark.read.format("mongodb")
        .option("connection.uri", connection_uri)
        .option("database", database_name)
        .option("collection", collection_name)
        .load()
    )

    bronze_df = (
        source_df
        .withColumn("_source_database", lit(database_name))
        .withColumn("_source_collection", lit(collection_name))
        .withColumn("_ingested_at", current_timestamp())
    )

    (
        bronze_df.write
        .format("delta")
        .mode("append")
        .option("mergeSchema", "true")
        .saveAsTable(f"`{catalog}`.bronze.{table_name}")
    )
