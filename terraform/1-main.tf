provider "azurerm" {}

resource "azurerm_resource_group" "fnrg" {
  name     = "function-rg"
  location = "${var.location}"
}

resource "azurerm_storage_account" "fnsa" {
  name                      = "releasrfnsa"
  resource_group_name       = "${azurerm_resource_group.fnrg.name}"
  location                  = "${var.location}"
  account_tier              = "standard"
  account_replication_type  = "lrs"
  enable_https_traffic_only = "true"
  account_kind              = "StorageV2"
}

resource "azurerm_storage_table" "fntable" {
  name                 = "functiontable"
  resource_group_name  = "${azurerm_resource_group.fnrg.name}"
  storage_account_name = "${azurerm_storage_account.fnsa.name}"
}

resource "azurerm_storage_queue" "fnsaqueue" {
  name                 = "functionqueue"
  resource_group_name  = "${azurerm_resource_group.fnrg.name}"
  storage_account_name = "${azurerm_storage_account.fnsa.name}"
}

resource "azurerm_app_service_plan" "fnasp" {
  name                = "releasr-fn-asp"
  resource_group_name = "${azurerm_resource_group.fnrg.name}"
  location            = "${var.location}"

  sku {
    tier = "Premium"
    size = "P1V2"
  }
}

resource "azurerm_function_app" "fnapp1" {
  name                      = "cpl-webapi-fn"
  resource_group_name       = "${azurerm_resource_group.fnrg.name}"
  location                  = "${var.location}"
  app_service_plan_id       = "${azurerm_app_service_plan.fnasp.id}"
  storage_connection_string = "${azurerm_storage_account.fnsa.primary_connection_string}"
  version                   = "~2"

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY = "${azurerm_application_insights.fnai.instrumentation_key}"
    WEBSITE_RUN_FROM_PACKAGE = 1
  }
}

resource "azurerm_function_app" "fnapp2" {
  name                      = "cpl-messaging-fn"
  resource_group_name       = "${azurerm_resource_group.fnrg.name}"
  location                  = "${var.location}"
  app_service_plan_id       = "${azurerm_app_service_plan.fnasp.id}"
  storage_connection_string = "${azurerm_storage_account.fnsa.primary_connection_string}"
  version                   = "~2"

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY = "${azurerm_application_insights.fnai.instrumentation_key}"
    WEBSITE_RUN_FROM_PACKAGE = 1
    AzureWebJobsServiceBus         = "${azurerm_servicebus_namespace_authorization_rule.fnsbnpolicy.primary_connection_string}"
    CosmosDBConnection             = "${azurerm_cosmosdb_account.cosmos.connection_strings[0]}"
  }
}

resource "azurerm_function_app" "fnapp" {
  name                      = "cpl-durable-fn"
  resource_group_name       = "${azurerm_resource_group.fnrg.name}"
  location                  = "${var.location}"
  app_service_plan_id       = "${azurerm_app_service_plan.fnasp.id}"
  storage_connection_string = "${azurerm_storage_account.fnsa.primary_connection_string}"
  version                   = "~2"

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY = "${azurerm_application_insights.fnai.instrumentation_key}"
    WEBSITE_RUN_FROM_PACKAGE = 1
  }
}

resource "azurerm_function_app" "fnapp3" {
  name                      = "cpl-spa-fn"
  resource_group_name       = "${azurerm_resource_group.fnrg.name}"
  location                  = "${var.location}"
  app_service_plan_id       = "${azurerm_app_service_plan.fnasp.id}"
  storage_connection_string = "${azurerm_storage_account.fnsa.primary_connection_string}"
  version                   = "~2"

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY = "${azurerm_application_insights.fnai.instrumentation_key}"
    WEBSITE_RUN_FROM_PACKAGE = 1
  }
}

resource "azurerm_application_insights" "fnai" {
  name                = "releasrfn-ai"
  resource_group_name = "${azurerm_resource_group.fnrg.name}"
  location            = "eastus"
  application_type    = "web"
}

resource "azurerm_servicebus_namespace" "fnsbn" {
  name                = "releasr-sn"
  resource_group_name = "${azurerm_resource_group.fnrg.name}"
  location            = "${var.location}"
  sku                 = "standard"
}

resource "azurerm_servicebus_topic" "fntopic" {
  name                = "functiontop"
  resource_group_name = "${azurerm_resource_group.fnrg.name}"
  namespace_name      = "${azurerm_servicebus_namespace.fnsbn.name}"
  enable_partitioning = true
}

resource "azurerm_servicebus_namespace_authorization_rule" "fnsbnpolicy" {
  name                = "functionpolicy"
  namespace_name      = "${azurerm_servicebus_namespace.fnsbn.name}"
  resource_group_name = "${azurerm_resource_group.fnrg.name}"
  listen              = true
  send                = true
  manage              = false
}

resource "azurerm_servicebus_subscription" "fnsub" {
  name                = "functionsub"
  resource_group_name = "${azurerm_resource_group.fnrg.name}"
  namespace_name      = "${azurerm_servicebus_namespace.fnsbn.name}"
  topic_name          = "${azurerm_servicebus_topic.fntopic.name}"
  max_delivery_count  = 1
}


resource "azurerm_cosmosdb_account" "cosmos" {
  name = "releasrcosmos"
  resource_group_name = "${azurerm_resource_group.fnrg.name}"
  location            = "${var.location}"
  offer_type          = "Standard"
  enable_automatic_failover = false

  consistency_policy {
    consistency_level = "Eventual"
  }

  geo_location {
    location          = "${var.location}"
    failover_priority = 0
  }
}