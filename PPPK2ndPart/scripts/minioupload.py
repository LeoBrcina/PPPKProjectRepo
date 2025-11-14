import os
from minio import Minio
from minio.error import S3Error

# SECOND SCRIPT

minio_client = Minio(
    "", 
    access_key="", 
    secret_key="",  
    secure=False 
)

BUCKET_NAME = "tcga-gene-storage"  
DATASET_DIR = "C:\\Users\\Korisnik\\Desktop\\Datasets"  
FOLDER_NAME = "tsv_files"  

def ensure_bucket_exists():
    try:
        if not minio_client.bucket_exists(BUCKET_NAME):
            minio_client.make_bucket(BUCKET_NAME)
            print(f"Bucket '{BUCKET_NAME}' created successfully.")
        else:
            print(f"Bucket '{BUCKET_NAME}' already exists.")
    except S3Error as e:
        print(f"Error checking/creating bucket: {e}")
        return False
    return True

def upload_files_to_minio():
    if not ensure_bucket_exists():
        return

    print("Checking for .tsv files in the dataset folder...")

    files_to_upload = os.listdir(DATASET_DIR) 

    if not files_to_upload:
        print("No .tsv files found! Check your path:", DATASET_DIR)
        return

    print(f"Found {len(files_to_upload)} .tsv files to upload:", files_to_upload)

    print("Starting file upload process...")

    for filename in files_to_upload:
        file_path = os.path.join(DATASET_DIR, filename)
        object_name = f"{FOLDER_NAME}/{filename}"

        print(f"Preparing to upload: {filename}...")

        
        try:
            minio_client.stat_object(BUCKET_NAME, object_name)
            print(f"{filename} already exists in MinIO. Skipping.")
            continue
        except S3Error:
            pass 

        try:
            minio_client.fput_object(BUCKET_NAME, object_name, file_path)
            print(f"{filename} uploaded successfully!")
        except Exception as e:
            print(f"Upload failed for {filename}: {e}")

if __name__ == "__main__":
    upload_files_to_minio()
