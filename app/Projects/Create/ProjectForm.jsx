"use client";
import { useState, useEffect } from "react";

export default function ProjectForm({ onSaved }) {
  // Basic project data
  const [projectNumber, setProjectNumber] = useState("");
  const [name, setName] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [statusId, setStatusId] = useState("");
  const [totalPrice, setTotalPrice] = useState("");
  const [description, setDescription] = useState("");

  // **Customers**: pick existing or create new
  const [customerId, setCustomerId] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [contactPerson, setContactPerson] = useState("");

  // **Services**
  const [selectedServiceId, setSelectedServiceId] = useState("");
  const [serviceName, setServiceName] = useState("");
  const [hourlyPrice, setHourlyPrice] = useState("");

  // **Staff**
  const [selectedStaffId, setSelectedStaffId] = useState("");
  const [staffName, setStaffName] = useState("");
  const [staffRole, setStaffRole] = useState("");

  // Dropdown data
  const [statuses, setStatuses] = useState([]);
  const [customers, setCustomers] = useState([]);
  const [services, setServices] = useState([]);
  const [staff, setStaff] = useState([]);

  // --- Fetch dropdown data ---
  useEffect(() => {
    async function fetchDropdownData() {
      try {
        const [statusRes, customerRes, serviceRes, staffRes] = await Promise.all([
          fetch("http://localhost:5127/api/projects/statuses"),
          fetch("http://localhost:5127/api/projects/customers"),
          fetch("http://localhost:5127/api/projects/services"),
          fetch("http://localhost:5127/api/projects/staff"),
        ]);

        if (!statusRes.ok || !customerRes.ok || !serviceRes.ok || !staffRes.ok) {
          throw new Error("One or more API endpoints are missing or invalid!");
        }

        const [statusesJson, customersJson, servicesJson, staffJson] = await Promise.all([
          statusRes.json(),
          customerRes.json(),
          serviceRes.json(),
          staffRes.json(),
        ]);

        setStatuses(statusesJson || []);
        setCustomers(customersJson || []);
        setServices(servicesJson || []);
        setStaff(staffJson || []);
      } catch (error) {
        console.error("Error fetching dropdown data:", error);
      }
    }

    fetchDropdownData();
  }, []);

  // --- Handle customer selection from dropdown ---
  function handleCustomerSelect(e) {
    const newCustomerId = e.target.value; // string
    setCustomerId(newCustomerId);

    // If found in the existing customers, fill in name/contact
    const chosen = customers.find((c) => c.customerId === parseInt(newCustomerId, 10));
    if (chosen) {
      setCustomerName(chosen.name);
      setContactPerson(chosen.contactPerson || "");
    } else {
      // If none selected, reset the new-customer fields
      setCustomerName("");
      setContactPerson("");
    }
  }

  // --- Handle service selection from dropdown ---
  function handleServiceChange(e) {
    const newServiceId = e.target.value;
    setSelectedServiceId(newServiceId);

    const chosen = services.find((s) => s.serviceId === parseInt(newServiceId, 10));
    if (chosen) {
      setServiceName(chosen.name);
      setHourlyPrice(chosen.hourlyPrice?.toString() || "0");
    } else {
      setServiceName("");
      setHourlyPrice("");
    }
  }

  // --- Handle staff selection from dropdown ---
  function handleStaffChange(e) {
    const newStaffId = e.target.value;
    setSelectedStaffId(newStaffId);

    const chosen = staff.find((s) => s.staffId === parseInt(newStaffId, 10));
    if (chosen) {
      setStaffName(chosen.name);
      setStaffRole(chosen.role?.name || "");
    } else {
      setStaffName("");
      setStaffRole("");
    }
  }

  // --- Handle "save" (POST to create) ---
  async function handleSave() {
    const parsedStatusId = parseInt(statusId, 10) || 0;
    const parsedCustomerId = customerId ? parseInt(customerId, 10) : 0;
    const parsedTotalPrice = parseFloat(totalPrice) || 0;
    const parsedHourlyPrice = parseFloat(hourlyPrice) || 0;
  
    // If user picked a dropdown ID
    const hasExistingCustomer = parsedCustomerId > 0;
  
    const projectData = {
      projectNumber,
      name,
      startDate,
      endDate,
      statusId: parsedStatusId,
      totalPrice: parsedTotalPrice,
      description,
      service: {
        serviceId: selectedServiceId ? parseInt(selectedServiceId, 10) : 0,
        name: serviceName,
        hourlyPrice: parsedHourlyPrice,
      },
      staff: {
        staffId: selectedStaffId ? parseInt(selectedStaffId, 10) : 0,
        name: staffName,
        roleName: staffRole,
      },
    };
  
    if (hasExistingCustomer) {
      // If the user picked an existing customer from the dropdown
      projectData.customerId = parsedCustomerId;
      // Optionally remove "customer" object so the server won't try to create a new one
    } else {
      // If user is creating a new customer, do NOT send "customerId"
      projectData.customer = {
        name: customerName,
        contactPerson: contactPerson || "",
      };
    }
  
    try {
      const response = await fetch("http://localhost:5127/api/projects/create-details", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(projectData),
      });
  
      if (!response.ok) {
        const errorText = await response.text();
        alert(`Error: ${errorText}`);
        return;
      }
  
      alert("Project created successfully!");
      if (onSaved) onSaved();
    } catch (error) {
      console.error("Error saving project:", error);
      alert("An error occurred while saving the project.");
    }
  }
  

  function handleCancel() {
    alert("Cancelled");
  }

  return (
    <div style={styles.container}>
      <h2>Skapa Nytt Projekt</h2>

      {/* Basic Project Fields */}
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Projektnr</label>
          <input
            style={styles.input}
            value={projectNumber}
            onChange={(e) => setProjectNumber(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Benämning</label>
          <input
            style={styles.input}
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
      </div>

      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Startdatum</label>
          <input
            style={styles.input}
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Slutdatum</label>
          <input
            style={styles.input}
            type="date"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
        </div>
      </div>

      <hr />
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Status</label>
          <select
            style={styles.input}
            value={statusId}
            onChange={(e) => setStatusId(e.target.value)}
          >
            <option value="">Välj status</option>
            {statuses.map((s) => (
              <option key={s.statusId} value={s.statusId}>
                {s.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Totalpris</label>
          <input
            style={styles.input}
            type="number"
            value={totalPrice}
            onChange={(e) => setTotalPrice(e.target.value)}
          />
        </div>
      </div>

      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Beskrivning</label>
          <textarea
            style={styles.textarea}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>
      </div>

      <hr />
      <h3>Kund</h3>
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Välj kund (befintlig)</label>
          <select style={styles.input} value={customerId} onChange={handleCustomerSelect}>
            <option value="">Välj kund</option>
            {customers.map((c) => (
              <option key={c.customerId} value={c.customerId}>
                {c.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Nytt Kundnamn</label>
          <input
            style={styles.input}
            value={customerName}
            onChange={(e) => setCustomerName(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Kontaktperson</label>
          <input
            style={styles.input}
            value={contactPerson}
            onChange={(e) => setContactPerson(e.target.value)}
          />
        </div>
      </div>

      <hr />
      <h3>Välj Tjänst</h3>
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Befintlig Tjänst</label>
          <select style={styles.input} value={selectedServiceId} onChange={handleServiceChange}>
            <option value="">Välj tjänst</option>
            {services.map((svc) => (
              <option key={svc.serviceId} value={svc.serviceId}>
                {svc.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Tjänstnamn (ny eller befintlig)</label>
          <input
            style={styles.input}
            value={serviceName}
            onChange={(e) => setServiceName(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Timpris</label>
          <input
            style={styles.input}
            type="number"
            value={hourlyPrice}
            onChange={(e) => setHourlyPrice(e.target.value)}
          />
        </div>
      </div>

      <hr />
      <h3>Välj Personal</h3>
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Befintlig Personal</label>
          <select style={styles.input} value={selectedStaffId} onChange={handleStaffChange}>
            <option value="">Välj personal</option>
            {staff.map((st) => (
              <option key={st.staffId} value={st.staffId}>
                {st.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Namn (ny eller befintlig)</label>
          <input
            style={styles.input}
            value={staffName}
            onChange={(e) => setStaffName(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Roll</label>
          <input
            style={styles.input}
            value={staffRole}
            onChange={(e) => setStaffRole(e.target.value)}
          />
        </div>
      </div>

      <div style={styles.buttonRow}>
        <button style={styles.cancelButton} onClick={handleCancel}>Avbryt</button>
        <button style={styles.saveButton} onClick={handleSave}>Spara</button>
      </div>
    </div>
  );
}

const styles = {
  container: { maxWidth: "800px", margin: "0 auto", padding: "20px", fontFamily: "Arial, sans-serif", backgroundColor: "#f8f8f8" },
  row: { display: "flex", flexWrap: "wrap", marginBottom: "15px" },
  column: { flex: "1 1 200px", marginRight: "20px", marginBottom: "10px", minWidth: "200px" },
  label: { display: "block", marginBottom: "5px", fontWeight: "bold" },
  input: { width: "100%", padding: "6px", boxSizing: "border-box" },
  textarea: { width: "100%", padding: "6px", height: "60px", boxSizing: "border-box" },
  buttonRow: { display: "flex", justifyContent: "flex-end", marginTop: "20px" },
  cancelButton: { backgroundColor: "#ccc", color: "#000", padding: "8px 16px", border: "none", marginRight: "10px", cursor: "pointer" },
  saveButton: { backgroundColor: "#4caf50", color: "#fff", padding: "8px 16px", border: "none", cursor: "pointer" },
};
