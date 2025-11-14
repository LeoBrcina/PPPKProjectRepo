import os
import pandas as pd
from minio import Minio
from minio.error import S3Error
from pymongo import MongoClient

# THIRD SCRIPT

MINIO_CLIENT = Minio(
    "",  
    access_key="", 
    secret_key="",  
    secure=False  
)

BUCKET_NAME = "tcga-gene-storage" 
FOLDER_NAME = "tsv_files"  
LOCAL_DOWNLOAD_DIR = "C:\\Users\\Korisnik\\Desktop\\MinIO_Downloads"  

MONGO_URI = "" 
DB_NAME = ""  
COLLECTION_NAME = ""  

CGAS_STING_GENES = [
    "C6orf150", "CCL5", "CXCL10", "TMEM173", "CXCL9", "CXCL11",
    "NFKB1", "IKBKE", "IRF3", "TREX1", "ATM", "IL6", "IL8"
]

if not os.path.exists(LOCAL_DOWNLOAD_DIR):
    os.makedirs(LOCAL_DOWNLOAD_DIR)

mongo_client = MongoClient(MONGO_URI)
db = mongo_client[DB_NAME]
collection = db[COLLECTION_NAME]

def process_files_from_minio():
    try:
        objects = MINIO_CLIENT.list_objects(BUCKET_NAME, prefix=FOLDER_NAME, recursive=True)

        for obj in objects:
            file_name = obj.object_name.split("/")[-1]
            local_file_path = os.path.join(LOCAL_DOWNLOAD_DIR, file_name)

            print(f"Downloading {file_name} from MinIO...")
            MINIO_CLIENT.fget_object(BUCKET_NAME, obj.object_name, local_file_path)
            print(f"Downloaded: {local_file_path}")

            df = pd.read_csv(local_file_path, sep="\t")

            df_filtered = df[df["sample"].isin(CGAS_STING_GENES)]

            documents = []
            for _, row in df_filtered.iterrows():
                gene_name = row["sample"]
                for patient_id, expression_value in row.items():
                    if patient_id != "sample":  
                        documents.append({
                            "patient_id": patient_id,
                            "cancer_cohort": file_name.replace(".tsv", ""), 
                            "gene_name": gene_name,
                            "expression_value": expression_value
                        })

            if documents:
                collection.insert_many(documents)
                print(f"Inserted {len(documents)} gene expression records into MongoDB.")

    except Exception as e:
        print(f"Error processing files: {e}")

if __name__ == "__main__":
    process_files_from_minio()
