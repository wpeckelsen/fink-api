# Fink API

Fink API is a backend service that demonstrates a **product discovery and comparison feature** for modern consumer applications.

Fink is positioned as a **modular feature prototype** that can be integrated into larger platforms (e.g. retail, health, or shopping apps).

---

## 🧠 Core Idea

Fink showcases a simple but powerful pipeline:

1. Scan a product barcode  
2. Fetch structured product data  
3. Generate and compare vector embeddings  
4. Return similar or alternative products  

This transforms a basic barcode scan into a **smart product comparison experience**.

---

## 🔍 How It Works

### 1. Barcode Input
A barcode is sent to the API as a string.

### 2. Product Lookup
- The API first checks the local database  
- If the product is not found, it fetches data from OpenFoodFacts  
- The product is then stored for future requests  

### 3. Similarity Search
- The product is embedded into a vector space  
- A similarity search is performed against existing products  
- The API returns a list of related items  

---

## 🧩 Similar Product Matching

Fink uses vector similarity to group products by relevance:

- **> 0.9 similarity** → Near-identical  
  _Example: same product, different packaging_

- **0.7 – 0.9 similarity** → Comparable alternatives  
  _Example: same category, different brand_

This enables a **lightweight recommendation and comparison engine** on top of standard product data.

---

## 🛒 Use Case

Fink is designed to support features like:

- Smart product comparison  
- Substitution suggestions  
- Price-aware decision making (future)  
- Enhanced barcode scanning experiences  

---

## 📦 API Responsibilities

The API currently handles:

- Barcode-based product retrieval  
- External product data ingestion  
- Persistent storage of product data  
- Vector-based similarity search  

---

## 📍 Future Extensions

Planned directions for the project include:

- **Store-level clustering**  
  Group products by physical retail locations  

- **Price tracking**  
  Track and compare price changes over time  

- **Geographic filtering**  
  Show relevant products based on user location  

- **Frontend integration**  
  Connect to a mobile or web client  

---

## ⚙️ Tech Stack

- .NET 8 (Web API)  
- SQLite  
- Azure Cloud  
- OpenFoodFacts API  
- Azure AI Search / Vector Embeddings  
- AI-assisted development through Github Copilot

---

## 🚀 Project Positioning

Fink is not intended to be a full product on its own.  
Instead, it serves as a **proof of concept for intelligent product discovery**, demonstrating how:

> A simple barcode scan can evolve into a contextual, recommendation-driven user experience.