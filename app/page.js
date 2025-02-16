"use client";

import { useState, useEffect } from "react";

export default function HomePage() {
  const [projectCount, setProjectCount] = useState(0);

  useEffect(() => {
    async function fetchProjectCount() {
      try {
        const res = await fetch("http://localhost:5127/api/projects");
        
        // Debugging: Log response status
        console.log("Response Status:", res.status);
  
        if (!res.ok) {
          throw new Error(`API request failed with status: ${res.status}`);
        }
  
        const data = await res.json();
        
        // Debugging: Log the response data
        console.log("Fetched Data:", data);
  
        setProjectCount(data.length);
      } catch (error) {
        console.error("Kunde inte hämta projekt:", error);
      }
    }
  
    fetchProjectCount();
  }, []);
  

  return (
    <div style={{ padding: "2rem", textAlign: "center" }}>
      <h1>Välkommen till Projektportalen</h1>
      <p>Antal projekt i systemet: <strong>{projectCount}</strong></p>

      <div style={{ marginTop: "1rem" }}>
        <a href="/Projects" style={{ marginRight: "1rem", fontSize: "1.2rem" }}>
          📂 Alla Projekt
        </a>
        <a href="/Projects/Create" style={{ fontSize: "1.2rem" }}>
          ➕ Skapa Nytt Projekt
        </a>
      </div>
    </div>
  );
}
