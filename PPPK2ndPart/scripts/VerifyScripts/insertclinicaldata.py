import pandas as pd
from pymongo import MongoClient

MONGO_URI = ""  # 🔹 Replace with your connection string
DB_NAME = "tcga_data"
COLLECTION_NAME = "clinical_data"

client = MongoClient(MONGO_URI)
db = client[DB_NAME]
collection = db[COLLECTION_NAME]

clinical_data_path = "C:\\Users\\Korisnik\\Desktop\\TCGA_clinical_survival_data (1).tsv"  # 🔹 Update if needed
df = pd.read_csv(clinical_data_path, sep="\t")

clinical_documents = []
for _, row in df.iterrows():
    clinical_documents.append({
        "bcr_patient_barcode": row["bcr_patient_barcode"],
        "DSS": row.get("DSS", None),  
        "OS": row.get("OS", None),  
        "clinical_stage": row.get("clinical_stage", None)
    })

if clinical_documents:
    collection.insert_many(clinical_documents)
    print(f"✅ Inserted {len(clinical_documents)} clinical records into MongoDB.")

print(f"📊 Total Clinical Records in MongoDB: {collection.count_documents({})}")
