import matplotlib.pyplot as plt
import seaborn as sns
from pymongo import MongoClient

# FOURTH SCRIPT

MONGO_URI = ""  
DB_NAME = ""
COLLECTION_NAME = ""

client = MongoClient(MONGO_URI)
db = client[DB_NAME]
collection = db[COLLECTION_NAME]

def fetch_gene_expression(gene_name, cohort=None):
    query = {"gene_name": gene_name}
    if cohort:
        query["cancer_cohort"] = cohort  

    cursor = collection.find(query, {"_id": 0, "patient_id": 1, "expression_value": 1})
    return list(cursor)

def plot_gene_expression(gene_name, cohort=None):
    print(f"Fetching data for gene: {gene_name}...")

    data = fetch_gene_expression(gene_name, cohort)
    
    if not data:
        print("No data found for the given gene or cohort.")
        return

    patients = [entry["patient_id"] for entry in data]
    expressions = [entry["expression_value"] for entry in data]

    plt.figure(figsize=(12, 6))
    sns.barplot(x=patients, y=expressions, hue=patients, palette="coolwarm", legend=False)

    plt.xlabel("Patient ID")
    plt.ylabel("Gene Expression Level")
    plt.title(f"Gene Expression of {gene_name}" + (f" in {cohort}" if cohort else ""))
    plt.xticks(rotation=90)
    plt.show()

if __name__ == "__main__":
    gene_name = input("Enter gene name (e.g., IRF3, TMEM173): ").strip()
    cohort = input("Enter cancer cohort (optional, press Enter to skip): ").strip() # FOR EXAMPLE: TCGA.ACC.sampleMap%2FHiSeqV2_PANCAN
                                                                                    # TCGA.BLCA.sampleMap%2FHiSeqV2_PANCAN
                                                                                    # TCGA.BRCA.sampleMap%2FHiSeqV2_PANCAN
    plot_gene_expression(gene_name, cohort if cohort else None)
