# 🐤 Fink API

**Fink API** is the backend for a grocery price-tracking app.  
It powers the future mobile frontend (built in **Expo**) and provides product, price, and store data via REST endpoints.

---

## 🛒 Overview

Fink lets users scan grocery barcodes to instantly view:

- **Price per kilo/liter/unit** 💰  
- **Similar products** and their prices across nearby stores 🧩  
- **Price history** for any tracked item 📈  

It’s designed for users who want to compare everyday grocery prices — for example, tracking different brands of coffee beans or monitoring how the same product’s price changes across supermarkets ☕️.

---

## 🔍 Product Scanning

When a user scans a barcode, the app:

1. Sends the barcode (as a string) to **Fink API** 📦  
2. Looks up the product in **Fink’s Azure SQL** database 🗂️  
3. If not found, the API calls the **OpenFoodFacts API** to fetch product details 🌐  
4. Saves the product in Fink’s database for faster future lookups 💾  

---

## 🧠 Related Products

Fink uses **vector similarity search** to find related or duplicate products 🧮.  
This allows results to be grouped by *closeness*:

- **>0.9 similarity** → near-identical (e.g. same coffee, different packaging) 🔁  
- **0.7–0.9 similarity** → comparable substitutes (e.g. same type of product, different brand) 🔄  

This feature makes Fink more than a simple barcode scanner — it’s a lightweight **comparison engine** ☕️.

---

## 📍 Nearby Stores

The API records basic user location data (longitude and latitude).  
In future versions, products will be clustered by store location so users can filter results to specific supermarkets or regions 🏪.

---

## ⚙️ Tech Stack

- **.NET 8.0** (Web API) 🧰  
- **SQLite** 🗄️  
- **Azure Cloud Deployment** ☁️  
- **Bicep** (Infrastructure as Code) 🧱  
- **OpenFoodFacts API** (Product data source) 🌍  
- **Vector embeddings / Azure AI Search** (for product similarity) 🧮  
- **OAuth** (Authentication) 🔐  
- **Codex** (AI-assisted coding) 🤖  

---

## 🚀 Future Goals

- Introduce clustering for store locations 🗺️  
- Integrate price tracking and visualization 📊 