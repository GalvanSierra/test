# ITM-Tickets Infrastructure

## Option 1: Local Development with Kind

```bash
# Create cluster
kind create cluster --config kind-config.yaml

# Load Docker images into kind
kind load docker-image danielvillamizara/itm-gateway-api:latest
kind load docker-image danielvillamizara/itm-inventory-api:latest
kind load docker-image danielvillamizara/itm-order-api:latest
kind load docker-image danielvillamizara/itm-price-api:latest
kind load docker-image danielvillamizara/itm-product-api:latest
kind load docker-image danielvillamizara/itm-notification-api:latest
kind load docker-image danielvillamizara/itm-search-api:latest

# Deploy infrastructure (Redis, RabbitMQ, etc.)
kubectl apply -f ../../docker-compose.yml  # or use Helm

# Deploy microservices
kubectl apply -f ../../inventory-deployment.yaml
kubectl apply -f ../../gateway-deployment.yaml
kubectl apply -f ../../product-deployment.yaml
kubectl apply -f ../../price-deployment.yaml
kubectl apply -f ../../notification-deployment.yaml
kubectl apply -f ../../search-deployment.yaml
kubectl apply -f ../../hpa.yaml
kubectl apply -f ../../itm-ingress.yaml
```

## Option 2: Azure AKS (Production)

```bash
# Login to Azure
az login

# Initialize Terraform
terraform init

# Preview changes
terraform plan

# Apply infrastructure
terraform apply -auto-approve

# Get kubeconfig
az aks get-credentials --resource-group itm-tickets-rg --name itm-tickets-aks

# Deploy microservices
kubectl apply -f ../../inventory-deployment.yaml
kubectl apply -f ../../gateway-deployment.yaml
# ... repeat for all services
```
