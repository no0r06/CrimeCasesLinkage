.

🧠 Crime Cases Linkage System
📌 Overview

This project is a similarity-based crime case analysis system designed to help identify potential links between criminal cases using structured behavioral and physical attributes.

Instead of manually reviewing past cases, the system automates similarity detection using a progressive series of algorithms (V0 → V4), each improving performance, efficiency, and scalability.

The core idea is:

Given a new crime case, retrieve and rank historically similar cases based on shared behavioral and victim-related attributes.

🎯 Problem Statement

Investigators manually comparing crime cases face:

High cognitive load when reviewing large datasets
Missed connections between similar cases
Inconsistent memory-based pattern recognition
Lack of scalability across large crime databases

This system addresses these limitations by introducing an algorithmic similarity matching pipeline.

🧠 Core Idea (Logic Chain)

All versions of the system are built around the same principle:

A crime case similarity score can be computed by comparing structured attributes such as:

Method of crime
Location
Victim gender
Victim age range overlap

Each version improves how comparisons are performed, not the scoring model itself.

🔄 Algorithm Evolution (V0 → V4)
🟢 V0 — Brute Force Similarity
Logic:
Compare every case with every other case
Compute similarity score based on all attributes
Sort results
Characteristics:
High accuracy
Very slow (O(N²))
No optimization
🔵 V1 — Method-Based Grouping
Logic Improvement:
Reduce comparisons by grouping cases by method first
Impact:
Less unnecessary comparisons
Faster than V0
Still relatively heavy
🟡 V2 — Multi-Feature Grouping (Gender + Location)
Logic Improvement:
Group cases using stronger filters (location + gender)
Impact:
Smaller comparison groups
Lower false positives
Better real-world clustering behavior
🟠 V3 — Indexed Filtering Optimization
Logic Improvement:
Use database filtering before computation
Reduce dataset size before similarity scoring
Impact:
Major reduction in comparisons
Faster execution time
More efficient data retrieval
🔴 V4 — Input-Based Top-K Ranking System
Logic Improvement:
Accepts a new incoming case as input
Filters candidates using indexed attributes
Computes similarity only on reduced dataset
Maintains Top-K best matches dynamically
Key Features:
Real-world query-based system
Ranking instead of full output
Controlled output size
Improved usability
⚙️ Similarity Scoring Model

All versions use the same scoring logic:

Method match → +1
Location match → +1
Victim gender match → +1
Age range overlap → +1

Maximum score depends on matching attributes.

📊 Performance Summary
Version	Strategy	Correct Complexity	Explanation
V0	Compare input vs all cases	O(N)	Single pass over dataset
V1	Method filtering + input compare	O(N)	Still scanning dataset, but minor filtering
V2	Multi-feature grouping	O(N) (smaller constant)	Same scan, better pre-grouping
V3	Indexed filtering	O(M)	M << N due to database filtering
V4	Indexed + Top-K ranking	O(M log K)	Maintain heap/Top-K instead of full sort
🧠 Key Design Philosophy

This project is not about changing the scoring model.

It is about improving:

Search space reduction
Computational efficiency
Practical usability
Result relevance control

Each version improves one bottleneck at a time, forming a clear optimization chain.

🚀 Final Insight

The system demonstrates that:

Performance improvements in real-world systems often come from reducing the effective dataset and controlling output, not just optimizing algorithms mathematically.

🛠️ Tech Stack
ASP.NET Core Web API
Entity Framework Core
SQL Server
LINQ
C#
📌 How to Run
Clone repository
Configure database connection string
Run migrations
Start API via Visual Studio
Test endpoints via Swagger / Postman / Scalar
📈 Future Improvements
Weighted similarity scoring (feature importance tuning)
Machine learning-based similarity prediction
Precomputed indexing tables
Graph-based crime linkage model
