$ErrorActionPreference = "Stop"

$root = $PSScriptRoot
Set-Location $root

$mongoUri = [Environment]::GetEnvironmentVariable("TUNAENGINE_MONGODB_URI")
if ([string]::IsNullOrWhiteSpace($mongoUri)) {
    throw "Set TUNAENGINE_MONGODB_URI to the MongoDB Atlas connection string before deploying."
}

$services = @{
    "TunaEngine.MembersService" = "tunaengine-members-service:local"
    "TunaEngine.EventsService" = "tunaengine-events-service:local"
    "TunaEngine.DocumentsService" = "tunaengine-documents-service:local"
    "TunaEngine.InventoryService" = "tunaengine-inventory-service:local"
    "TunaEngine.Gateway" = "tunaengine-gateway:local"
}

foreach ($service in $services.Keys) {
    $image = $services[$service]
    Write-Host "Building $service as $image..."
    docker build --tag $image `
        --file "$root\$service\Dockerfile" "$root\$service"
}

kubectl config use-context docker-desktop

kubectl apply -f "$root\k8s\namespace.yaml"
kubectl apply -f "$root\k8s\configmap.yaml"

kubectl create secret generic mongodb-secret `
    --namespace tunaengine `
    --from-literal="MongoDb__ConnectionString=$mongoUri" `
    --dry-run=client -o yaml |
    kubectl apply -f -

kubectl apply -f "$root\k8s\deployments.yaml"
kubectl apply -f "$root\k8s\services.yaml"
kubectl apply -f "$root\k8s\ingress.yaml"

kubectl rollout status deployment/gateway -n tunaengine --timeout=180s
kubectl rollout status deployment/members-service -n tunaengine --timeout=180s
kubectl rollout status deployment/events-service -n tunaengine --timeout=180s
kubectl rollout status deployment/documents-service -n tunaengine --timeout=180s
kubectl rollout status deployment/inventory-service -n tunaengine --timeout=180s
