# Terraform configuration for AKS (Azure Kubernetes Service)
# This is provided as a reference for cloud deployment.
# For local development, use `kind` with kind-config.yaml instead.

terraform {
  required_version = ">= 1.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_kubernetes_cluster" "main" {
  name                = var.cluster_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  dns_prefix          = var.cluster_name

  default_node_pool {
    name       = "default"
    node_count = var.node_count
    vm_size    = "Standard_B2s"
  }

  identity {
    type = "SystemAssigned"
  }

  tags = {
    Environment = "Development"
    Project     = "ITM-Tickets"
  }
}

# Note: Using Docker Hub for container registry (configured via GitHub Actions secrets).
# If ACR is preferred, uncomment below:
# resource "azurerm_container_registry" "main" {
#   name                = replace(var.cluster_name, "-", "")
#   resource_group_name = azurerm_resource_group.main.name
#   location            = azurerm_resource_group.main.location
#   sku                 = "Basic"
#   admin_enabled       = true
# }
