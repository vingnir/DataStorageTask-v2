// components/ProjectsList.jsx
import styles from "../styles/ProjectList.module.css";

export default function ProjectsList({ projects, onEdit, onDelete }) {
  if (!projects?.length) {
    return <p className={styles.noData}>Inga projekt att visa</p>;
  }

  return (
    <table className={styles.table}>
      <thead>
        <tr>
          <th>Projektnr</th>
          <th>Benämning</th>
          <th>Kundnamn</th>
          <th>Kontakt person</th>
          <th>Startdatum</th>
          <th>Slutdatum</th>
          <th>Status</th>
          <th>Price/hour</th>
          <th>Service</th>
          <th>Assigned to project</th>
          <th>Role</th>
          <th>Actions</th> {/* ✅ New column for buttons */}
        </tr>
      </thead>
      <tbody>
        {projects.map((proj) => (
          <tr key={proj.projectNumber}>
            <td>{proj.projectNumber}</td>
            <td>{proj.name}</td>
            <td>{proj.customerName}</td>
            <td>{proj.contactPerson}</td>
            <td>{proj.startDate?.substring(0, 10)}</td>
            <td>{proj.endDate?.substring(0, 10)}</td>
            <td>{proj.statusName}</td>
            <td>{proj.service?.hourlyPrice || "N/A"}</td>
            <td>{proj.service?.name || "N/A"}</td>
            <td>{proj.staff?.name || "N/A"}</td>
            <td>{proj.staff?.roleName || "N/A"}</td>
            <td>
              {/* ✅ Edit Button */}
              <button onClick={() => onEdit(proj)} style={{ background: "blue", color: "white", border: "none", padding: "5px 10px", cursor: "pointer", marginRight: "5px" }}>
                ✏ Edit
              </button>
              {/* ✅ Delete Button */}
              <button onClick={() => onDelete(proj.projectNumber)} style={{ background: "red", color: "white", border: "none", padding: "5px 10px", cursor: "pointer" }}>
                🗑 Delete
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
