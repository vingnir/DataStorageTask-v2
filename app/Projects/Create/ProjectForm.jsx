"use client";
import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";


export default function ProjectForm({ onSaved }) {

  const router = useRouter();

  
  const [projectNumber, setProjectNumber] = useState("");
  const [name, setName] = useState("");
  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [statusId, setStatusId] = useState("");
  const [totalPrice, setTotalPrice] = useState("");
  const [description, setDescription] = useState("");


  const [customerId, setCustomerId] = useState("");
  const [customerName, setCustomerName] = useState("");
  const [contactPerson, setContactPerson] = useState("");

  
  const [selectedServiceId, setSelectedServiceId] = useState("");
  const [serviceName, setServiceName] = useState("");
  const [hourlyPrice, setHourlyPrice] = useState("");

 
  const [selectedStaffId, setSelectedStaffId] = useState("");
  const [staffName, setStaffName] = useState("");
  const [staffRole, setStaffRole] = useState("");

 
  const [statuses, setStatuses] = useState([]);
  const [customers, setCustomers] = useState([]);
  const [services, setServices] = useState([]);
  const [staff, setStaff] = useState([]);

  // From copilot auto suggestion
  //  Fetch dropdown data from API then checks if it OK, if it is OK it will convert the data to JSON and set the data to the state
  // Also handles errors
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
    
      const result = await response.json(); // Get server response
    
      if (!response.ok) {
        console.error("Server Error:", result);
        alert(`Error: ${result.message || "Failed to save project."}`);
        return;
      }
    
      alert("Project created successfully!");
      if (onSaved) onSaved();
      router.push("/Projects");
    } catch (error) {
      console.error("Error saving project:", error);
      alert("An unexpected error occurred.");
    }
    
  }
  

  // --- Handle "cancel" ---
  function handleCancel() {
    if (confirm("Are you sure you want to cancel?")) {
      router.push("/");
    }
  }

  return (
    <div style={styles.container}>
      <h2>Create a new Project</h2>

      {/* Basic Project Fields */}
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Project Number</label>
          <input
            style={styles.input}
            value={projectNumber}
            onChange={(e) => setProjectNumber(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Label</label>
          <input
            style={styles.input}
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
      </div>

      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Start Date</label>
          <input
            style={styles.input}
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>End Date</label>
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
            <option value="">Select status</option>
            {statuses.map((s) => (
              <option key={s.statusId} value={s.statusId}>
                {s.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Total Price</label>
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
          <label style={styles.label}>Description</label>
          <textarea
            style={styles.textarea}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </div>
      </div>

      <hr />
      <h3>Customer</h3>
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Select/Create Customer</label>
          <select style={styles.input} value={customerId} onChange={handleCustomerSelect}>
            <option value="">New Customer</option>
            {customers.map((c) => (
              <option key={c.customerId} value={c.customerId}>
                {c.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>New Customer name</label>
          <input
            style={styles.input}
            value={customerName}
            onChange={(e) => setCustomerName(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>New Contact Person</label>
          <input
            style={styles.input}
            value={contactPerson}
            onChange={(e) => setContactPerson(e.target.value)}
          />
        </div>
      </div>

      <hr />
      <h3>Service</h3>
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}> Select/Create Service</label>
          <select style={styles.input} value={selectedServiceId} onChange={handleServiceChange}>
            <option value="">New Service</option>
            {services.map((svc) => (
              <option key={svc.serviceId} value={svc.serviceId}>
                {svc.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>New Service</label>
          <input
            style={styles.input}
            value={serviceName}
            onChange={(e) => setServiceName(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Hourly Price</label>
          <input
            style={styles.input}
            type="number"
            value={hourlyPrice}
            onChange={(e) => setHourlyPrice(e.target.value)}
          />
        </div>
      </div>

      <hr />
      <h3>Staff</h3>
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Select/Create Staff</label>
          <select style={styles.input} value={selectedStaffId} onChange={handleStaffChange}>
            <option value="">New Staff</option>
            {staff.map((st) => (
              <option key={st.staffId} value={st.staffId}>
                {st.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>New Name</label>
          <input
            style={styles.input}
            value={staffName}
            onChange={(e) => setStaffName(e.target.value)}
          />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>New Role</label>
          <input
            style={styles.input}
            value={staffRole}
            onChange={(e) => setStaffRole(e.target.value)}
          />
        </div>
      </div>

      <div style={styles.buttonRow}>
        <button style={styles.cancelButton} onClick={handleCancel}>Cancel</button>
        <button style={styles.saveButton} onClick={handleSave}>Save</button>
      </div>
    </div>
  );
}

const styles = {
  container: { maxWidth: "800px", margin: "0 auto", padding: "20px", fontFamily: "Arial, sans-serif", backgroundColor: "#005a38", color: "#ffcc00" },
  row: { display: "flex", flexWrap: "wrap", marginBottom: "15px" },
  column: { flex: "1 1 200px", marginRight: "20px", marginBottom: "10px", minWidth: "200px" },
  label: { display: "block", marginBottom: "5px", fontWeight: "bold" },
  input: { width: "100%", padding: "6px", boxSizing: "border-box" },
  textarea: { width: "100%", padding: "6px", height: "60px", boxSizing: "border-box" },
  buttonRow: { display: "flex", justifyContent: "flex-end", marginTop: "20px" },
  cancelButton: { backgroundColor: "#ccc", color: "#000", padding: "8px 16px", border: "none", marginRight: "10px", cursor: "pointer" },
  saveButton: { backgroundColor: "#f9e700", color: "#161616", padding: "8px 16px", border: "none", cursor: "pointer" },
  saveButtonHover: { backgroundColor: "#ffee02" }
};