# PPPK Project – Medical Records WebApp & Genomics ETL

This repository contains a complete PPPK course project that combines:

- An **ASP.NET Core MVC web application** for managing patients, medical records, appointments, prescriptions, medicines, diseases and attachments (PostgreSQL + Entity Framework Core).
- A set of **Python ETL & analysis scripts** for working with **TCGA / genomics data**, using **Selenium**, **MinIO**, **MongoDB**, and plotting tools.

> ⚠️ All real credentials (PostgreSQL, MongoDB, MinIO) have been cleared from the repo.  
> You must provide your own connection strings and access keys before running anything.

---

## Table of Contents

1. [Project Structure](#project-structure)  
2. [Tech Stack](#tech-stack)  
3. [WebApp (ASP.NET Core MVC)](#webapp-aspnet-core-mvc)  
   - [Prerequisites](#prerequisites)  
   - [Configuration](#configuration)  
   - [Running the WebApp](#running-the-webapp)  
4. [Python Scripts (PPPK2ndPart)](#python-scripts-pppk2ndpart)  
   - [Prerequisites](#python-prerequisites)  
   - [Environment Setup](#environment-setup)  
   - [Scripts Overview](#scripts-overview)  
   - [Running the Scripts](#running-the-scripts)  
5. [Database Model (WebApp)](#database-model-webapp)  
6. [Security Notes](#security-notes)  
7. [License](#license)

---

## Project Structure

    PPPKProjectRepo/
    ├── PPPK_LB/
    │   ├── PPPK_LB.sln
    │   └── WebApp/
    │       ├── Controllers/
    │       │   ├── AppointmentController.cs
    │       │   ├── AttachmentController.cs
    │       │   ├── DiseaseController.cs
    │       │   ├── HomeController.cs
    │       │   ├── MedicalRecordController.cs
    │       │   ├── MedicineController.cs
    │       │   ├── PatientController.cs
    │       │   └── PrescriptionController.cs
    │       ├── Models/
    │       │   ├── Appointment.cs
    │       │   ├── Appointmenttype.cs
    │       │   ├── Attachment.cs
    │       │   ├── Disease.cs
    │       │   ├── Medicalrecord.cs
    │       │   ├── Medicine.cs
    │       │   ├── Patient.cs
    │       │   ├── Prescription.cs
    │       │   └── PostgresContext.cs
    │       ├── ViewModels/
    │       ├── Views/
    │       │   ├── Appointment/…
    │       │   ├── Attachment/…
    │       │   ├── Disease/…
    │       │   ├── Home/…
    │       │   ├── MedicalRecord/…
    │       │   ├── Medicine/…
    │       │   ├── Patient/…
    │       │   ├── Prescription/…
    │       │   └── Shared/…
    │       ├── Migrations/
    │       ├── wwwroot/
    │       ├── appsettings.json
    │       ├── appsettings.Development.json
    │       └── Program.cs
    └── PPPK2ndPart/
        └── scripts/
            ├── VerifyScripts/
            │   ├── geneverify.py
            │   └── insertclinicaldata.py
            ├── miniotomongo.py
            ├── minioupload.py
            ├── scraper.py
            └── visualisation.py

---

## Tech Stack

**WebApp**

- ASP.NET Core MVC  
- Entity Framework Core (code-first)  
- PostgreSQL  
- Bootstrap-based UI  

**Python / Data**

- Python 3.x  
- Selenium (TCGA/Xena-style scraper)  
- MinIO Python client  
- `pymongo` (MongoDB)  
- `pandas` (data handling)  
- `matplotlib`, `seaborn` (visualisation)  

**Other**

- MongoDB (for TCGA-like gene expression & clinical data)  
- MinIO object storage (datasets)  

---

## WebApp (ASP.NET Core MVC)

### Prerequisites

- **.NET SDK** compatible with the `WebApp.csproj` TargetFramework (e.g. recent .NET 7/8 SDK).  
- A running **PostgreSQL** instance (local or remote).  
- An IDE such as **Visual Studio 2022** or **Rider**, or the `dotnet` CLI.  

### Configuration

1. **Clone the repository**

       git clone https://github.com/LeoBrcina/PPPKProjectRepo.git
       cd PPPKProjectRepo/PPPK_LB/WebApp

2. **Configure PostgreSQL connection string**

   Open `appsettings.json` and/or `appsettings.Development.json` and set your connection string:

       {
         "ConnectionStrings": {
           "PostgresConnection": "Host=localhost;Port=5432;Database=pppk_db;Username=your_user;Password=your_password"
         }
       }

   Make sure the name (`PostgresConnection`) matches the one used in `PostgresContext` / `Program.cs`.

3. **Apply migrations (optional but recommended)**

   From the `WebApp` folder:

       dotnet tool install --global dotnet-ef   # if needed
       dotnet ef database update

   Alternatively, EF Core can create/update the database on first run depending on how `PostgresContext` is configured.

### Running the WebApp

You can run the application either from an IDE or via CLI.

**Using CLI:**

    cd PPPKProjectRepo/PPPK_LB/WebApp
    dotnet restore
    dotnet run

The app will start on a URL similar to:

    https://localhost:5001

Then open it in a browser and you should see the home page with navigation to:

- Patients  
- Medical Records  
- Appointments  
- Prescriptions  
- Medicines  
- Diseases  
- Attachments  

---

## Python Scripts (PPPK2ndPart)

The `PPPK2ndPart/scripts` folder contains standalone Python utilities that handle:

- **Scraping gene expression data** from an online source.  
- **Uploading raw data to MinIO**.  
- **Importing processed data into MongoDB**.  
- **Verifying and enriching datasets** with clinical metadata.  
- **Visualising gene expression** from MongoDB with simple plots.  

### Python Prerequisites

- Python **3.10+** (recommended)  
- A running **MongoDB** instance  
- A running **MinIO** instance (or other S3-compatible storage) for dataset files  
- Optional: **Chrome/Firefox + WebDriver** for Selenium scraping  

### Environment Setup

From the repo root:

    cd PPPKProjectRepo/PPPK2ndPart/scripts

Create and activate a virtual environment:

    python -m venv .venv
    # Windows:
    .venv\Scripts\activate
    # Linux/macOS:
    source .venv/bin/activate

Install dependencies (if there is no `requirements.txt`, install the typical ones):

    pip install pymongo minio selenium pandas matplotlib seaborn

> 🔐 All connection strings (`MONGO_URI`, MinIO keys, etc.) have been cleared.  
> Open each script and configure the constants or environment variables before running.

### Scripts Overview

#### `scraper.py`

- Uses **Selenium** to open a data portal (e.g. Xena Browser).  
- Iterates over cohorts, downloads selected gene expression datasets.  
- Saves them to a configurable folder (e.g. `C:\Users\Korisnik\Desktop\Datasets`).  

Key tasks:

- Configure:
  - Download directory  
  - Path to WebDriver (ChromeDriver/GeckoDriver)  

Run it to automatically download cohorts of interest.

---

#### `minioupload.py`

- Connects to **MinIO** using the Python client.  
- Creates a bucket (if needed).  
- Uploads raw dataset files (e.g. downloaded by `scraper.py`) to MinIO.  

You need to set:

- MinIO endpoint (host + port)  
- Access key / secret key  
- Bucket name  
- Local path to datasets  

---

#### `miniotomongo.py`

- Reads data files (optionally from MinIO).  
- Parses gene expression tables.  
- Inserts documents into **MongoDB**, typically using:
  - Database like `tcga_data`  
  - Collection like `gene_expressions`  

Typical document fields:

- `gene_name`  
- `cancer_cohort`  
- `patient_id`  
- `expression_value`  

---

#### `VerifyScripts/geneverify.py`

- Verifies **gene IDs / names** loaded into MongoDB.  
- Can be used to check if a gene exists in a given cohort or validate dataset integrity.  

Typical responsibilities:

- Connects to MongoDB.  
- Runs queries by `gene_name` / `cancer_cohort`.  
- Prints basic stats or warnings for missing/inconsistent data.  

---

#### `VerifyScripts/insertclinicaldata.py`

- Inserts **clinical metadata** (e.g. patient-level info) into MongoDB.  
- Likely reads from CSV files and writes to a dedicated collection (e.g. `clinical_data`).  
- Used to enrich gene expression data with clinical attributes for later analysis.  

---

#### `visualisation.py`

Interactive plotting of gene expression from MongoDB.

- Connects to MongoDB and the gene expression database.  
- Uses a collection (by default) like `gene_expressions`.  
- Provides two main functions (or equivalent logic):
  - Fetch expression by `gene_name` and optional `cohort`.  
  - Plot expression values using `matplotlib` / `seaborn`.  

Example document in MongoDB:

    {
      "gene_name": "IRF3",
      "cancer_cohort": "TCGA.ACC.sampleMap%2FHiSeqV2_PANCAN",
      "patient_id": "TCGA-XXXX-YYYY",
      "expression_value": 5.123
    }

### Running the Scripts

From `PPPK2ndPart/scripts` (with venv activated):

#### 1. Scrape datasets

    python scraper.py

Check constants (start URL, download folder) at the top/bottom of the file and adjust to your environment.

#### 2. Upload to MinIO

    python minioupload.py

Make sure MinIO endpoint and credentials are configured.

#### 3. Move data from MinIO to MongoDB

    python miniotomongo.py

Ensure MongoDB and MinIO settings are correct.

#### 4. Insert clinical data

    python VerifyScripts/insertclinicaldata.py

Configure input CSV paths and MongoDB connection.

#### 5. Verify gene data

    python VerifyScripts/geneverify.py

Use this to quickly sanity-check imported genes/cohorts.

#### 6. Visualise gene expression

    python visualisation.py

The script will typically prompt you for a gene name (and possibly a cohort) and then plot a chart of expression values per patient.

---

## Database Model (WebApp)

The ASP.NET WebApp uses **Entity Framework Core** with a `PostgresContext` that maps to the following tables/entities:

- `Patient` – core patient information.  
- `Medicalrecord` – medical history and records for each patient.  
- `Appointment` & `Appointmenttype` – scheduling and appointment types.  
- `Prescription` – prescribed medicines and dosages for patients.  
- `Medicine` – medicine catalog / lookup.  
- `Disease` – disease catalog / diagnosis codes.  
- `Attachment` – files attached to medical records (reports, imaging, etc.).  

Each entity has:

- A model in `WebApp/Models`  
- (Optionally) a ViewModel in `WebApp/ViewModels` for form handling / view composition  
- A controller for CRUD operations  
- Razor views in the corresponding folder under `WebApp/Views`  

Together they form a simple medical records management system with standard ASP.NET MVC patterns.

---

## Security Notes

- **All secrets are intentionally empty** in the repo:
  - PostgreSQL connection string  
  - `MONGO_URI`, `DB_NAME`, `COLLECTION_NAME` in Python scripts  
  - MinIO access/secret keys  

Before running the project:

1. Create your own **development database users** (Postgres, MongoDB, MinIO).  
2. Fill the connection values either:
   - Directly in config files (for local dev only), or  
   - Preferably via environment variables / user secrets in real setups.  
3. Never commit real credentials back to the repository.  

---

## License

This project is intended for **educational / coursework purposes**.

If you plan to publicly reuse or extend this project, please add an explicit license  
(e.g. MIT, Apache-2.0) or follow your faculty’s guidelines.
