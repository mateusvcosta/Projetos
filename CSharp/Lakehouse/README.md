# TunaEngine Lakehouse

Esta pasta contém a camada analítica do TunaEngine para execução no Databricks.
O TunaEngine continua a usar MongoDB para a operação diária; o lakehouse copia
os dados para Delta Lake e prepara métricas para análise.

## Arquitetura

```text
MongoDB do TunaEngine
        |
        v
Bronze: documentos brutos + metadados de ingestão
        |
        v
Silver: dados normalizados, deduplicados e validados
        |
        v
Gold: factos, dimensões e indicadores analíticos
        |
        v
Databricks SQL / dashboards / Power BI
```

## Componentes

- `notebooks/01_bronze_ingestion.py`: lê as coleções MongoDB e grava Delta;
- `notebooks/02_silver_transformations.py`: limpa e normaliza os dados;
- `sql/03_gold_analytics.sql`: cria as tabelas analíticas;
- `databricks.yml`: configuração opcional de um Databricks Asset Bundle.

## Configuração no Databricks

1. Criar um catálogo `tunaengine` ou alterar o widget `catalog` dos notebooks.
2. Criar um secret scope chamado `tunaengine`.
3. Guardar a URI do MongoDB no secret `connection-uri`.
4. Instalar no cluster o MongoDB Spark Connector compatível com a versão do
   Databricks Runtime. A dependência deve ser adicionada como biblioteca Maven
   pelo administrador do workspace.
5. Importar os dois notebooks e executar primeiro Bronze, depois Silver e por
   fim o script SQL Gold.

A URI é sempre obtida pelo Secrets API. Nunca deve ser colocada num notebook,
ficheiro de configuração, variável versionada ou dashboard.

## Modelo analítico Gold

- `gold.dim_member`: membros sem campos de autenticação;
- `gold.dim_inventory`: inventário de instrumentos;
- `gold.dim_event`: eventos;
- `gold.fact_event_participation`: participação dos membros nos eventos;
- `gold.fact_event_inventory`: instrumentos necessários por evento;
- `gold.fact_event_ride`: boleias, capacidade e passageiros por evento;
- `gold.kpi_event_overview`: visão consolidada de cada evento;
- `gold.kpi_event_attendance`: presenças e taxas de participação por evento;
- `gold.kpi_event_monthly`: evolução mensal por local;
- `gold.kpi_inventory_usage`: utilização do inventário em eventos.

## Perguntas analíticas

O caso de estudo de eventos permite responder, por exemplo:

- Quais os eventos com maior taxa de presença?
- Que locais têm mais eventos e maior participação?
- Quantos instrumentos são necessários por evento?
- Qual a utilização e a capacidade das boleias?
- Como evolui a participação ao longo dos meses?

## Execução recomendada

Os notebooks podem ser encadeados num Databricks Workflow com três tarefas:

```text
bronze_ingestion -> silver_transformations -> gold_analytics
```

As transformações Silver removem duplicados pelo identificador funcional.
Assim, a ingestão Bronze pode ser repetida sem publicar duplicados na camada
analítica.
