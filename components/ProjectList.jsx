import React, { useState } from "react";
import styles from "../styles/ProjectList.module.css";

export default function ProjectsList({ projects, onEdit, onDelete }) {
  const [expandedProject, setExpandedProject] = useState(null);

  if (!projects?.length) {
    return <p className={styles.noData}>No Projects Available</p>;
  }

  const toggleDropdown = (projectNumber) => {
    setExpandedProject(expandedProject === projectNumber ? null : projectNumber);
  };

  return (
    <table className={styles.table}>
      <thead>
        <tr>
          <th></th>
          <th>ProjectNr</th>
          <th>Customer</th>
          <th>Contact Person</th>
          <th>Start Date</th>
          <th>End Date</th>
          <th>Status</th>
          <th>Service</th>
          <th>Assigned to project</th>
          <th>Role</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {projects.map((proj) => {
          let statusClass = "";
          if (proj.statusName === "Completed") {
            statusClass = styles.completed;
          } else if (proj.statusName === "Ongoing") {
            statusClass = styles.inProgress;
          } else if (proj.statusName === "Not Started") {
            statusClass = styles.notStarted;
          }

          return (
            <React.Fragment key={`proj-${proj.projectNumber}`}>
              <tr key={`row-${proj.projectNumber}`} className={statusClass} onClick={() => toggleDropdown(proj.projectNumber)}>
                <td className={styles.arrowCell}>
                  <span className={styles.arrow}>{expandedProject === proj.projectNumber ? "▲" : "▼"}</span>
                </td>
                <td>{proj.projectNumber}</td>
                <td>{proj.customerName}</td>
                <td>{proj.contactPerson}</td>
                <td>{proj.startDate?.substring(0, 10)}</td>
                <td>{proj.endDate?.substring(0, 10)}</td>
                <td>{proj.statusName}</td>
                <td>{proj.service?.name || "N/A"}</td>
                <td>{proj.staff?.name || "N/A"}</td>
                <td>{proj.staff?.roleName || "N/A"}</td>
                <td>
                  <button onClick={() => onEdit(proj)} className={styles.editButton}>
                    ✏ Edit
                  </button>
                  <button onClick={() => onDelete(proj.projectNumber)} className={styles.deleteButton}>
                    🗑 Delete
                  </button>
                </td>
              </tr>
              {/* // Made with chatGpt 4o
              // Shows a dropdown with project details when the row is clicked */}
              {expandedProject === proj.projectNumber && (
                <tr className={styles.dropdownRow} key={`details-${proj.projectNumber}`}>
                  <td colSpan="14">
                    <div className={styles.dropdownContent}>
                      <p><strong>Description:</strong> {proj.description || "No description provided."}</p>
                      <p><strong>Label:</strong> {proj.name || "No label provided."}</p>
                      <p><strong>Price/hour:</strong> {proj.service?.hourlyPrice ? `${proj.service.hourlyPrice} kr` : "N/A"}</p>
                      <p><strong>Total Price:</strong> {proj.totalPrice ? `${proj.totalPrice} kr` : "N/A"}</p>
                    </div>
                  </td>
                </tr>
              )}
            </React.Fragment>
          );
        })}
      </tbody>
    </table>
  );
}
