# Kubernetes deployment

No Docker Desktop:

1. Abra Docker Desktop.
2. Em Settings, selecione Kubernetes.
3. Ative "Enable Kubernetes".
4. Aguarde o estado "Kubernetes is running".
5. Confirme:

```powershell
kubectl config use-context docker-desktop
kubectl get nodes
```

Build and publish the five images to a registry accessible by the cluster.

Update `mongodb-secret.yaml` before applying it. Do not commit real credentials.

Apply resources:

```powershell
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/
kubectl -n tunaengine get pods
```

The MongoDB StatefulSet creates a `PersistentVolumeClaim` through `volumeClaimTemplates`.
