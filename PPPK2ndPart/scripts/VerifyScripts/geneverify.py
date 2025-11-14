from pymongo import MongoClient

MONGO_URI = ""  # Replace with your connection string
client = MongoClient(MONGO_URI)
db = client["tcga_data"]
collection = db["gene_expressions"]

total_documents = collection.count_documents({})
print(f"📊 Total Documents: {total_documents}")

cancer_cohorts = collection.distinct("cancer_cohort")
print(f"📁 Cancer Cohorts: {cancer_cohorts}")

genes = collection.distinct("gene_name")
print(f"🧬 Genes Stored: {genes}")

sample_data = collection.aggregate([{ "$sample": { "size": 5 } }])
for doc in sample_data:
    print(doc)
