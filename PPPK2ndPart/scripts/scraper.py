import os
import requests
import gzip
import shutil
import time
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager

# FIRST SCRIPT - TAKES A LONG TIME

def setup_webdriver():
    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()))
    driver.maximize_window()
    return driver

def download_file(download_url, save_dir):
    response = requests.get(download_url, stream=True)
    filename = os.path.join(save_dir, download_url.split('/')[-1])

    with open(filename, 'wb') as file:
        for chunk in response.iter_content(chunk_size=1024):
            if chunk:
                file.write(chunk)

    print(f"Downloaded: {filename}")

    if filename.endswith('.gz'):
        extract_gz_file(filename, save_dir)

def extract_gz_file(gz_filepath, save_dir):
    extracted_filepath = os.path.join(save_dir, os.path.basename(gz_filepath).replace('.gz', ''))

    try:
        with gzip.open(gz_filepath, 'rb') as f_in:
            with open(extracted_filepath, 'wb') as f_out:
                shutil.copyfileobj(f_in, f_out)

        print(f"Extracted: {extracted_filepath}")

        os.remove(gz_filepath)
        print(f"Deleted: {gz_filepath}")

    except Exception as e:
        print(f"Error extracting {gz_filepath}: {e}")

def get_cohort_links(driver, wait):
    cohort_list = wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, 'ul.Datapages-module__list___2yM9o')))
    time.sleep(3)
    cohort_items = cohort_list.find_elements(By.TAG_NAME, 'li')
    return [item.find_element(By.TAG_NAME, 'a') for item in cohort_items]

def download_illumina_data(driver, wait, save_dir):
    try:
        illumina_link = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, 'IlluminaHiSeq pancan normalized')))
        illumina_link.click()
    except Exception:
        try:
            illumina_link = wait.until(EC.element_to_be_clickable((By.LINK_TEXT, 'polyA+ IlluminaHiSeq pancan normalized')))
            illumina_link.click()
        except Exception:
            return False

    try:
        download_link = wait.until(EC.presence_of_element_located((By.XPATH, '//a[contains(text(), "download")]'))).get_attribute('href')
        download_file(download_link, save_dir)
    except Exception:
        return False

    return True

def scrape_cohorts(start_url, save_dir):
    if not os.path.exists(save_dir):
        os.makedirs(save_dir)

    driver = setup_webdriver()
    driver.get(start_url)
    wait = WebDriverWait(driver, 30)

    try:
        total_cohorts = len(get_cohort_links(driver, wait))

        for idx in range(total_cohorts):
            cohort_links = get_cohort_links(driver, wait)
            if idx >= len(cohort_links):
                break

            cohort_url = cohort_links[idx].get_attribute('href')
            driver.get(cohort_url)

            try:
                if not download_illumina_data(driver, wait, save_dir):
                    pass

                driver.get(start_url)
                wait.until(EC.presence_of_element_located((By.CSS_SELECTOR, 'ul.Datapages-module__list___2yM9o')))
                time.sleep(3)

            except Exception:
                pass

    except Exception:
        pass
    finally:
        driver.quit()

if __name__ == "__main__":
    START_URL = 'https://xenabrowser.net/datapages/?hub=https://tcga.xenahubs.net:443'
    SAVE_DIR = 'C:\\Users\\Korisnik\\Desktop\\Datasets'
    scrape_cohorts(START_URL, SAVE_DIR)
