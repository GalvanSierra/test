variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "eastus"
}

variable "resource_group_name" {
  description = "Name of the Azure resource group"
  type        = string
  default     = "itm-tickets-rg"
}

variable "cluster_name" {
  description = "Name of the AKS cluster"
  type        = string
  default     = "itm-tickets-aks"
}

variable "node_count" {
  description = "Number of AKS worker nodes"
  type        = number
  default     = 3
}
