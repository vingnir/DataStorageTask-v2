"use client";

import { useState, useEffect } from "react";
import styles from "./HomePage.module.css"; // Import the CSS module

export default function HomePage() {
  const [projectCount, setProjectCount] = useState(0);

  useEffect(() => {
    async function fetchProjectCount() {
      try {
        const res = await fetch("http://localhost:5127/api/projects");
        
        console.log("Response Status:", res.status);
  
        if (!res.ok) {
          throw new Error(`API request failed with status: ${res.status}`);
        }
  
        const data = await res.json();
        
        console.log("Fetched Data:", data);
  
        setProjectCount(data.length);
      } catch (error) {
        console.error("Kunde inte hämta projekt:", error);
      }
    }
  
    fetchProjectCount();
  }, []);
  

  return (
    <div className={styles.container}>
      <h1>Pavado´s Development</h1>
      <p>Number of projects: <strong>{projectCount}</strong></p>

      <div className={styles.links}>
        <a href="/Projects" className={styles.link1}>
          📂 All projects
        </a>
        <a href="/Projects/Create" className={styles.link2}>
          ➕ Create a new project
        </a>
      </div>
    </div>
  );
}
