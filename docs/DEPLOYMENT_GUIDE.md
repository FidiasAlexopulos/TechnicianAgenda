# 🚀 Guía de Deployment - Agenda de Técnicos

## Arquitectura Recomendada (AWS)

```
┌─────────────────────────────────────────────────────────┐
│                      USUARIOS                           │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│              CloudFront (CDN Global)                     │
└─────────────────────────────────────────────────────────┘
         │                              │
         │                              │
         ▼                              ▼
┌──────────────────┐        ┌──────────────────────┐
│  S3 Bucket       │        │  Application Load    │
│  (React App)     │        │  Balancer            │
└──────────────────┘        └──────────────────────┘
                                       │
                                       ▼
                            ┌──────────────────────┐
                            │  Elastic Beanstalk   │
                            │  (.NET API)          │
                            └──────────────────────┘
                                       │
                ┌──────────────────────┼──────────────────────┐
                │                      │                      │
                ▼                      ▼                      ▼
        ┌──────────────┐      ┌──────────────┐      ┌──────────────┐
        │   RDS SQL    │      │ ElastiCache  │      │  S3 Bucket   │
        │   Server     │      │   (Redis)    │      │  (Uploads)   │
        └──────────────┘      └──────────────┘      └──────────────┘
```

## 📋 Paso a Paso - Deployment AWS

### Fase 1: Preparación (Local)

#### 1.1 Preparar el Backend
```bash
# En tu carpeta de backend
cd TechnicianAgenda

# Crear archivo para AWS Elastic Beanstalk
# Archivo: aws-windows-deployment-manifest.json
```

#### 1.2 Preparar el Frontend
```bash
# En tu carpeta de React
cd technician-agenda-ui

# Crear build de producción
npm run build

# Esto genera la carpeta /build con tu app optimizada
```

### Fase 2: Configurar AWS

#### 2.1 Crear cuenta AWS
1. Ve a https://aws.amazon.com
2. Crear cuenta (necesitas tarjeta de crédito, pero hay capa gratuita)
3. Instalar AWS CLI: https://aws.amazon.com/cli/

#### 2.2 Configurar AWS CLI
```bash
aws configure
# AWS Access Key ID: [Tu access key]
# AWS Secret Access Key: [Tu secret key]
# Default region: us-east-1
# Default output format: json
```

### Fase 3: Deploy Base de Datos

#### 3.1 Crear RDS SQL Server
```bash
# Via AWS Console o CLI
aws rds create-db-instance \
    --db-instance-identifier technician-agenda-db \
    --db-instance-class db.t3.micro \
    --engine sqlserver-ex \
    --master-username admin \
    --master-user-password [TU_PASSWORD_SEGURO] \
    --allocated-storage 20
```

#### 3.2 Migrar datos
```bash
# Desde tu SQL Server local
# 1. Generar script de schema
# 2. Conectar a RDS y ejecutar
# 3. Opcional: Migrar datos existentes
```

### Fase 4: Deploy Redis Cache

#### 4.1 Crear ElastiCache Redis
```bash
aws elasticache create-cache-cluster \
    --cache-cluster-id technician-cache \
    --cache-node-type cache.t3.micro \
    --engine redis \
    --num-cache-nodes 1
```

### Fase 5: Deploy Backend (.NET API)

#### 5.1 Instalar EB CLI
```bash
pip install awsebcli
```

#### 5.2 Inicializar y Deploy
```bash
cd TechnicianAgenda

# Inicializar Elastic Beanstalk
eb init -p "64bit Windows Server 2022 v2.11.0 running IIS 10.0" -r us-east-1

# Crear ambiente
eb create technician-api-env

# Deploy
eb deploy
```

### Fase 6: Deploy Frontend (React)

#### 6.1 Crear bucket S3
```bash
aws s3 mb s3://technician-agenda-frontend
```

#### 6.2 Configurar bucket para hosting
```bash
aws s3 website s3://technician-agenda-frontend \
    --index-document index.html \
    --error-document index.html
```

#### 6.3 Subir archivos
```bash
cd technician-agenda-ui/build
aws s3 sync . s3://technician-agenda-frontend --acl public-read
```

#### 6.4 Configurar CloudFront (Opcional pero recomendado)
- Mejora velocidad global
- HTTPS automático
- Cache inteligente

### Fase 7: Configurar Variables de Entorno

#### 7.1 En Elastic Beanstalk
```bash
eb setenv \
    ConnectionStrings__DefaultConnection="Server=tu-rds-endpoint;Database=TechnicianAgendaDB;..." \
    Redis__ConnectionString="tu-elasticache-endpoint:6379" \
    ASPNETCORE_ENVIRONMENT="Production"
```

#### 7.2 En React (archivo .env.production)
```env
REACT_APP_API_URL=https://tu-api-eb-url.elasticbeanstalk.com/api
```

### Fase 8: Storage de Archivos (S3)

#### 8.1 Crear bucket para uploads
```bash
aws s3 mb s3://technician-agenda-uploads
```

#### 8.2 Modificar backend para usar S3
- Instalar paquete: `AWSSDK.S3`
- Modificar upload de archivos para guardar en S3

---

## 💰 Costos Estimados (AWS)

### Capa Gratuita (12 meses):
- ✅ EC2: 750 horas/mes (t2.micro)
- ✅ RDS: 750 horas/mes (db.t2.micro)
- ✅ S3: 5GB storage, 20,000 GET requests
- ✅ ElastiCache: 750 horas/mes (cache.t2.micro)

### Después de capa gratuita (~$50-100/mes):
- EC2/Elastic Beanstalk: $15-30
- RDS SQL Server: $20-40
- ElastiCache Redis: $15-20
- S3 + CloudFront: $5-10
- Data Transfer: $5-10

---

## 🔐 Seguridad

### Checklist de seguridad:
- [ ] HTTPS habilitado (CloudFront/ELB)
- [ ] Base de datos NO pública (VPC privada)
- [ ] Redis NO público (VPC privada)
- [ ] Secrets en AWS Secrets Manager
- [ ] IAM Roles con permisos mínimos
- [ ] CORS configurado correctamente
- [ ] Rate limiting en API
- [ ] Backup automático de DB habilitado

---

## 🚀 Alternativa Rápida: Railway.app (Para Demo)

Si quieres algo más rápido para demostrar:

### Backend en Railway
```bash
# 1. Instalar Railway CLI
npm i -g @railway/cli

# 2. Login
railway login

# 3. Iniciar proyecto
railway init

# 4. Deploy
railway up
```

### Frontend en Vercel
```bash
# 1. Instalar Vercel CLI
npm i -g vercel

# 2. Deploy desde carpeta de React
cd technician-agenda-ui
vercel --prod
```

**Ventajas:**
- ⚡ Deploy en 5 minutos
- 💰 Plan gratuito generoso
- 🔄 Auto-deploy con Git
- 📊 Dashboard fácil

**Desventajas:**
- ⚠️ Menos control
- ⚠️ No SQL Server (tendrías que migrar a PostgreSQL)

---

## 📚 Recursos Adicionales

### Documentación:
- AWS Elastic Beanstalk .NET: https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/dotnet-core-tutorial.html
- AWS S3 Static Hosting: https://docs.aws.amazon.com/AmazonS3/latest/userguide/WebsiteHosting.html
- Railway Docs: https://docs.railway.app

### Videos Recomendados:
- "Deploy .NET API to AWS" - freeCodeCamp
- "Deploy React to AWS S3" - Traversy Media
- "AWS ElastiCache Tutorial" - AWS Official

---

## 🎯 Mi Recomendación

**Para aprender y demo rápido:**
1. Frontend → Vercel (gratis, súper fácil)
2. Backend → Railway o Render (gratis con limitaciones)
3. DB → Supabase PostgreSQL (gratis, tendrías que migrar de SQL Server)

**Para proyecto serio/producción:**
1. Todo en AWS como describí arriba
2. Cuesta ~$50-100/mes después de capa gratuita
3. Pero es escalable y profesional

---

## 📝 Próximos Pasos

1. ¿Quieres ir por la ruta AWS (profesional) o algo más rápido para demo?
2. ¿Tienes cuenta de AWS?
3. ¿Quieres que te ayude con algún paso específico?

Puedo crear scripts de deployment automatizados para ti.
