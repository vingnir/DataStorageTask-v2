"use client";

import { useState, useEffect } from "react";
import ProjectsList from "@/components/ProjectList"; // ✅ Ensure correct import
import EditProjectForm from "./edit/EditProjectForm"; // ✅ Ensure correct import
import styles from "@/styles/ProjectList.module.css";

export default function ProjectsPage() {
  const [projects, setProjects] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [editingProject, setEditingProject] = useState(null); // ✅ Track which project is being edited

  useEffect(() => {
    fetchProjects();
  }, []);

  async function fetchProjects() {
    try {
      const res = await fetch("http://localhost:5127/api/projects");
      if (!res.ok) throw new Error("Failed to fetch projects");
      const data = await res.json();
      setProjects(data);
    } catch (err) {
      console.error("Error fetching projects:", err);
    }
  }

  async function deleteProject(projectNumber) {
    if (!confirm(`Are you sure you want to delete project ${projectNumber}?`)) return;

    try {
      const res = await fetch(`http://localhost:5127/api/projects/${projectNumber}`, {
        method: "DELETE"
      });

      if (res.ok) {
        alert("Project deleted successfully.");
        setProjects(projects.filter((proj) => proj.projectNumber !== projectNumber));
      } else {
        alert("Failed to delete project.");
      }
    } catch (error) {
      alert("Error deleting project: " + error.message);
    }
  }

  function startEditingProject(project) {
    setEditingProject(project);
  }

  async function updateProject(updatedProject) {
    try {
      const res = await fetch(`http://localhost:5127/api/projects/${updatedProject.projectNumber}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(updatedProject),
      });

      if (res.ok) {
        alert("Project updated successfully.");
        setEditingProject(null);
        fetchProjects();
      } else {
        alert("Failed to update project.");
      }
    } catch (error) {
      alert("Error updating project: " + error.message);
    }
  }

  const filteredProjects = projects.filter((proj) => {
    const pn = proj.projectNumber?.toLowerCase() || "";
    const cust = proj.customerName?.toLowerCase() || "";
    return pn.includes(searchText.toLowerCase()) || cust.includes(searchText.toLowerCase());
  });

  return (
    <div className={styles.container}>
      <header className={styles.header}>
        <h1>Alla Projekt</h1>
        <div>
          <label>Sök:</label>
          <input
            type="text"
            placeholder="Sök på projektnr eller kundnamn"
            value={searchText}
            onChange={(e) => setSearchText(e.target.value)}
            className={styles.searchInput}
          />
        </div>
      </header>

      {editingProject ? (
        <EditProjectForm project={editingProject} onUpdate={updateProject} onCancel={() => setEditingProject(null)} />
      ) : (
        <ProjectsList 
          projects={filteredProjects} 
          onDelete={deleteProject}  // ✅ Pass `onDelete`
          onEdit={startEditingProject}  // ✅ Pass `onEdit`
        />
      )}
    </div>
  );
}
