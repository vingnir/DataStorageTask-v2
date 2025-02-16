"use client";
import { useState, useEffect } from "react";

export default function EditProjectForm({ project, onUpdated, onCancelled }) {
  // Top-level project fields
  const [projectNumber, setProjectNumber] = useState(project.projectNumber);
  const [name, setName] = useState(project.name || "");
  const [startDate, setStartDate] = useState(project.startDate || "");
  const [endDate, setEndDate] = useState(project.endDate || "");
  const [statusId, setStatusId] = useState(project.statusId?.toString() || "");
  const [totalPrice, setTotalPrice] = useState(project.totalPrice?.toString() || "");
  const [description, setDescription] = useState(project.description || "");

  // Customer fields
  // For existing customer selection:
  const [customerId, setCustomerId] = useState(project.customerId?.toString() || "");
  // For new customer creation:
  const [customerName, setCustomerName] = useState(project.customer?.name || "");
  const [contactPerson, setContactPerson] = useState(project.customer?.contactPerson || "");

  // Service fields
  const [selectedServiceId, setSelectedServiceId] = useState(project.service?.serviceId?.toString() || "");
  const [serviceName, setServiceName] = useState(project.service?.name || "");
  const [hourlyPrice, setHourlyPrice] = useState(project.service?.hourlyPrice?.toString() || "");

  // Staff fields
  const [selectedStaffId, setSelectedStaffId] = useState(project.staff?.staffId?.toString() || "");
  const [staffName, setStaffName] = useState(project.staff?.name || "");
  const [staffRole, setStaffRole] = useState(project.staff?.roleName || "");

  // Dropdown data arrays
  const [statuses, setStatuses] = useState([]);
  const [customers, setCustomers] = useState([]);
  const [services, setServices] = useState([]);
  const [staff, setStaff] = useState([]);

  // --- Fetch dropdown data (Status, Customer, Services, Staff) ---
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
          throw new Error("One or more API endpoints failed!");
        }

        const [statusesJson, customersJson, servicesJson, staffJson] = await Promise.all([
          statusRes.json().catch(() => []),
          customerRes.json().catch(() => []),
          serviceRes.json().catch(() => []),
          staffRes.json().catch(() => []),
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

  // --- Handler for customer dropdown ---
  function handleCustomerSelect(e) {
    const newId = e.target.value;
    setCustomerId(newId);
    const chosen = customers.find((c) => c.customerId === parseInt(newId, 10));
    if (chosen) {
      setCustomerName(chosen.name);
      setContactPerson(chosen.contactPerson || "");
    } else {
      setCustomerName("");
      setContactPerson("");
    }
  }

  // --- Handler for service dropdown ---
  function handleServiceChange(e) {
    const newId = e.target.value;
    setSelectedServiceId(newId);
    const chosen = services.find((s) => s.serviceId === parseInt(newId, 10));
    if (chosen) {
      setServiceName(chosen.name);
      setHourlyPrice(chosen.hourlyPrice?.toString() || "0");
    } else {
      setServiceName("");
      setHourlyPrice("");
    }
  }

  // --- Handler for staff dropdown ---
  function handleStaffChange(e) {
    const newId = e.target.value;
    setSelectedStaffId(newId);
    const chosen = staff.find((s) => s.staffId === parseInt(newId, 10));
    if (chosen) {
      setStaffName(chosen.name);
      setStaffRole(chosen.role?.name || "");
    } else {
      setStaffName("");
      setStaffRole("");
    }
  }

  // --- Handle form submission (PUT to update project) ---
  async function handleUpdate() {
  // Parse numeric fields
  const parsedStatusId = parseInt(statusId, 10) || 0;
  const parsedTotalPrice = parseFloat(totalPrice) || 0;
  const parsedHourlyPrice = parseFloat(hourlyPrice) || 0;
  const parsedCustomerId = customerId ? parseInt(customerId, 10) : 0;

  const updatedProject = {
    projectNumber,
    name,
    startDate,
    endDate,
    statusId: parsedStatusId,
    totalPrice: parsedTotalPrice,
    description,

    // Customer logic
    ...(parsedCustomerId > 0
      ? { customerId: parsedCustomerId }
      : {
          customer: {
            name: customerName?.trim() || "",
            contactPerson: contactPerson?.trim() || "",
          },
        }),

    // Service logic
    service: selectedServiceId && selectedServiceId > 0
      ? { serviceId: parseInt(selectedServiceId, 10) }
      : { name: serviceName?.trim() || "", hourlyPrice: parsedHourlyPrice },

    // Staff logic (ensuring it's not missing)
    staff: selectedStaffId && selectedStaffId > 0
  ? { staffId: parseInt(selectedStaffId, 10) } // Existing Staff
  : { name: staffName?.trim() || "", roleName: staffRole?.trim() || "" }, // New Staff

  };

  console.log("Updated Project Payload:", updatedProject); // ✅ Debugging

  try {
    const response = await fetch(`http://localhost:5127/api/projects/${projectNumber}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(updatedProject),
    });

    // Log full response to check for issues
    const responseText = await response.text();
    console.log("API Response:", response.status, responseText); // ✅ Debugging API response

    if (!response.ok) {
      console.error("API Error Response:", responseText);
      alert(`Error: ${responseText || "Could not update project"}`);
      return;
    }

    alert("Project updated successfully!");
  } catch (error) {
    console.error("Network error:", error);
    alert("Network error while updating the project.");
  }
}

  

  function handleCancel() {
    if (onCancelled) {
      onCancelled();
    } else {
      alert("Cancelled");
    }
  }

  return (
    <div style={styles.container}>
      <h2>Redigera Projekt</h2>

      {/* Top Row: Basic Project Data */}
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Projektnr</label>
          <input style={styles.input} value={projectNumber} disabled />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Benämning</label>
          <input style={styles.input} value={name} onChange={(e) => setName(e.target.value)} />
        </div>
      </div>

      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Startdatum</label>
          <input style={styles.input} type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Slutdatum</label>
          <input style={styles.input} type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
        </div>
      </div>

      {/* Status & Customer */}
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Status</label>
          <select style={styles.input} value={statusId} onChange={(e) => setStatusId(e.target.value)}>
            <option value="">Välj status</option>
            {statuses.map((s) => (
              <option key={s.statusId} value={s.statusId}>
                {s.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Kund (befintlig)</label>
          <select style={styles.input} value={customerId} onChange={handleCustomerSelect}>
            <option value="">Välj kund</option>
            {customers.map((c) => (
              <option key={c.customerId} value={c.customerId}>
                {c.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Price and Description */}
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Totalpris</label>
          <input style={styles.input} type="number" value={totalPrice} onChange={(e) => setTotalPrice(e.target.value)} />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Beskrivning</label>
          <textarea style={styles.textarea} value={description} onChange={(e) => setDescription(e.target.value)} />
        </div>
      </div>

      <hr />
      {/* Customer Section for New Customer Data */}
      <h3>Kund (Ny eller befintlig)</h3>
      <div style={styles.row}>
        <div style={styles.column}>
          <label style={styles.label}>Välj kund</label>
          <select style={styles.input} value={customerId} onChange={handleCustomerSelect}>
            <option value="">--Välj en kund--</option>
            {customers.map((c) => (
              <option key={c.customerId} value={c.customerId}>
                {c.name}
              </option>
            ))}
          </select>
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Nytt Kundnamn</label>
          <input style={styles.input} value={customerName} onChange={(e) => setCustomerName(e.target.value)} />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Kontaktperson</label>
          <input style={styles.input} value={contactPerson} onChange={(e) => setContactPerson(e.target.value)} />
        </div>
      </div>

      <hr />
      {/* Service Section */}
      <h3>Tjänst (Ny eller befintlig)</h3>
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
          <input style={styles.input} value={serviceName} onChange={(e) => setServiceName(e.target.value)} />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Timpris</label>
          <input style={styles.input} type="number" value={hourlyPrice} onChange={(e) => setHourlyPrice(e.target.value)} />
        </div>
      </div>

      <hr />
      {/* Staff Section */}
      <h3>Personal (Ny eller befintlig)</h3>
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
          <input style={styles.input} value={staffName} onChange={(e) => setStaffName(e.target.value)} />
        </div>
        <div style={styles.column}>
          <label style={styles.label}>Roll</label>
          <input style={styles.input} value={staffRole} onChange={(e) => setStaffRole(e.target.value)} />
        </div>
      </div>

      <div style={styles.buttonRow}>
        <button style={styles.cancelButton} onClick={handleCancel}>
          Avbryt
        </button>
        <button style={styles.saveButton} onClick={handleUpdate}>
          Spara
        </button>
      </div>
    </div>
  );
}

// Move handleCustomerSelect inside the component so state is accessible
function handleCustomerSelect(e) {
  // In this example, this function should be defined inside the component.
  // If you're moving it outside, you'll need to pass state setters as arguments.
  // For simplicity, define it inside the component body.
  // (The above code already calls setCustomerId, etc., so remove this external function.)
}

const styles = {
  container: {
    maxWidth: "800px",
    margin: "0 auto",
    padding: "20px",
    fontFamily: "Arial, sans-serif",
    backgroundColor: "#f8f8f8",
  },
  row: {
    display: "flex",
    flexWrap: "wrap",
    marginBottom: "15px",
  },
  column: {
    flex: "1 1 200px",
    marginRight: "20px",
    marginBottom: "10px",
    minWidth: "200px",
  },
  label: { display: "block", marginBottom: "5px", fontWeight: "bold" },
  input: { width: "100%", padding: "6px", boxSizing: "border-box" },
  textarea: {
    width: "100%",
    padding: "6px",
    height: "60px",
    boxSizing: "border-box",
  },
  buttonRow: {
    display: "flex",
    justifyContent: "flex-end",
    marginTop: "20px",
  },
  cancelButton: {
    backgroundColor: "#ccc",
    color: "#000",
    padding: "8px 16px",
    border: "none",
    marginRight: "10px",
    cursor: "pointer",
  },
  saveButton: {
    backgroundColor: "#4caf50",
    color: "#fff",
    padding: "8px 16px",
    border: "none",
    cursor: "pointer",
  },
};





// "use client";
// import { useState, useEffect } from "react";

// export default function EditProjectForm({ project, onUpdated, onCancelled }) {
 

//   const [projectNumber, setProjectNumber] = useState(project.projectNumber);
//   const [name, setName] = useState(project.name || "");
//   const [startDate, setStartDate] = useState(project.startDate || "");
//   const [endDate, setEndDate] = useState(project.endDate || "");
//   const [statusId, setStatusId] = useState(project.statusId?.toString() || "");
//   const [customerId, setCustomerId] = useState(project.customerId?.toString() || "");
//   const [totalPrice, setTotalPrice] = useState(project.totalPrice?.toString() || "");
//   const [description, setDescription] = useState(project.description || "");


//   const [serviceName, setServiceName] = useState(project.service?.name || "");
//   const [hourlyPrice, setHourlyPrice] = useState(
//     project.service?.hourlyPrice?.toString() || ""
//   );

//   const [staffName, setStaffName] = useState(project.staff?.name || "");
//   const [staffRole, setStaffRole] = useState(project.staff?.roleName || "");


//   const [statuses, setStatuses] = useState([]);
//   const [customers, setCustomers] = useState([]);
//   const [services, setServices] = useState([]);
//   const [staff, setStaff] = useState([]);


//   const [selectedServiceId, setSelectedServiceId] = useState("");
//   const [selectedStaffId, setSelectedStaffId] = useState("");


//   useEffect(() => {
//     async function fetchDropdownData() {
//       try {
//         const [statusRes, customerRes, serviceRes, staffRes] = await Promise.all([
//           fetch("http://localhost:5127/api/projects/statuses"),
//           fetch("http://localhost:5127/api/projects/customers"),
//           fetch("http://localhost:5127/api/projects/services"),
//           fetch("http://localhost:5127/api/projects/staff"),
//         ]);

//         if (!statusRes.ok || !customerRes.ok || !serviceRes.ok || !staffRes.ok) {
//           throw new Error("One or more API endpoints failed!");
//         }

//         const [statusesJson, customersJson, servicesJson, staffJson] =
//           await Promise.all([
//             statusRes.json().catch(() => []),
//             customerRes.json().catch(() => []),
//             serviceRes.json().catch(() => []),
//             staffRes.json().catch(() => []),
//           ]);

//         setStatuses(statusesJson || []);
//         setCustomers(customersJson || []);
//         setServices(servicesJson || []);
//         setStaff(staffJson || []);
//       } catch (error) {
//         console.error("Error fetching dropdown data:", error);
//       }
//     }

//     fetchDropdownData();
//   }, []);


//   function handleServiceChange(e) {
//     const newId = e.target.value; 
//     setSelectedServiceId(newId);

//     const chosen = services.find((svc) => svc.serviceId === parseInt(newId, 10));
//     if (chosen) {
//       setServiceName(chosen.name);
//       setHourlyPrice(chosen.hourlyPrice?.toString() || "0");
//     } else {
//       setServiceName("");
//       setHourlyPrice("");
//     }
//   }

 
//   function handleStaffChange(e) {
//     const newId = e.target.value;
//     setSelectedStaffId(newId);

//     const chosen = staff.find((st) => st.staffId === parseInt(newId, 10));
//     if (chosen) {
//       setStaffName(chosen.name);
//       setStaffRole(chosen.role?.name || ""); 
//     } else {
//       setStaffName("");
//       setStaffRole("");
//     }
//   }

  
//   async function handleUpdate() {
//     const parsedStatusId = parseInt(statusId, 10) || 0;
//     const parsedCustomerId = parseInt(customerId, 10) || 0;
//     const parsedTotalPrice = parseFloat(totalPrice) || 0;
//     const parsedHourlyPrice = parseFloat(hourlyPrice) || 0;
    
   
//     const parsedStaffId = selectedStaffId
//       ? parseInt(selectedStaffId, 10)
//       : project.staff?.staffId || 0;
  
    
//     const parsedServiceId = selectedServiceId
//       ? parseInt(selectedServiceId, 10)
//       : project.service?.serviceId || 0;
  
    
//     if (!parsedStaffId || parsedStaffId === 0) {
//       alert("Error: Please select a valid Staff.");
//       return;
//     }
//     if (!parsedServiceId || parsedServiceId === 0) {
//       alert("Error: Please select a valid Service.");
//       return;
//     }
  
    
//     const updatedProject = {
//   projectNumber,
//   name,
//   startDate,
//   endDate,
//   statusId: parseInt(statusId, 10) || 0,
//   customerId: parseInt(customerId, 10) || 0,
//   totalPrice: parseFloat(totalPrice) || 0,
//   description,
//   serviceId: parseInt(selectedServiceId, 10) || project.service?.serviceId || 0, 
//   staffId: parsedStaffId, 
//   service: {
//     name: serviceName,
//     hourlyPrice: parseFloat(hourlyPrice) || 0,
//   },
//   staff: {
//     name: staffName,
//     roleName: staffRole,
//   },
// };

//     console.log("Updated Project Data:", JSON.stringify(updatedProject, null, 2));

//     try {
//       const response = await fetch(
//         `http://localhost:5127/api/projects/${encodeURIComponent(projectNumber)}`,
//         {
//           method: "PUT",
//           headers: { "Content-Type": "application/json" },
//           body: JSON.stringify(updatedProject),
//         }
//       );
  
//       if (!response.ok) {
//         const errorText = await response.text();
//         alert(`Error: ${errorText}`);
//         return;
//       }
  
//       alert("Project updated successfully!");
//       if (onUpdated) onUpdated();
//     } catch (error) {
//       console.error("Error updating project:", error);
//       alert("An error occurred while updating the project.");
//     }
//   }
  
  

//   return (
//     <div style={styles.container}>
//       <h2>Redigera Projekt</h2>

  
//       <div style={styles.row}>
//         <div style={styles.column}>
//           <label style={styles.label}>Projektnr</label>
//           <input
//             style={styles.input}
//             value={projectNumber}
//             disabled
//             onChange={(e) => setProjectNumber(e.target.value)}
//           />
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Benämning</label>
//           <input
//             style={styles.input}
//             value={name}
//             onChange={(e) => setName(e.target.value)}
//           />
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Startdatum</label>
//           <input
//             style={styles.input}
//             type="date"
//             value={startDate}
//             onChange={(e) => setStartDate(e.target.value)}
//           />
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Slutdatum</label>
//           <input
//             style={styles.input}
//             type="date"
//             value={endDate}
//             onChange={(e) => setEndDate(e.target.value)}
//           />
//         </div>
//       </div>

   
//       <div style={styles.row}>
//         <div style={styles.column}>
//           <label style={styles.label}>Status</label>
//           <select
//             style={styles.input}
//             value={statusId}
//             onChange={(e) => setStatusId(e.target.value)}
//           >
//             <option value="">Välj status</option>
//             {statuses.map((s) => (
//               <option key={s.statusId} value={s.statusId}>
//                 {s.name}
//               </option>
//             ))}
//           </select>
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Kund</label>
//           <select
//             style={styles.input}
//             value={customerId}
//             onChange={(e) => setCustomerId(e.target.value)}
//           >
//             <option value="">Välj kund</option>
//             {customers.map((c) => (
//               <option key={c.customerId} value={c.customerId}>
//                 {c.name}
//               </option>
//             ))}
//           </select>
//         </div>
//       </div>

      
//       <div style={styles.row}>
//         <div style={styles.column}>
//           <label style={styles.label}>Totalpris</label>
//           <input
//             style={styles.input}
//             type="number"
//             value={totalPrice}
//             onChange={(e) => setTotalPrice(e.target.value)}
//           />
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Beskrivning</label>
//           <textarea
//             style={styles.textarea}
//             value={description}
//             onChange={(e) => setDescription(e.target.value)}
//           />
//         </div>
//       </div>

//       <hr />
//       <h3>Uppdatera Tjänst</h3>
//       <div style={styles.row}>
//         <div style={styles.column}>
//           <label style={styles.label}>Välj från befintlig lista</label>
//           <select
//             style={styles.input}
//             value={selectedServiceId}
//             onChange={handleServiceChange}
//           >
//             <option value="">Välj tjänst</option>
//             {services.map((svc) => (
//               <option key={svc.serviceId} value={svc.serviceId}>
//                 {svc.name}
//               </option>
//             ))}
//           </select>
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Tjänst Namn</label>
//           <input
//             style={styles.input}
//             value={serviceName}
//             onChange={(e) => setServiceName(e.target.value)}
//           />
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Timpris</label>
//           <input
//             style={styles.input}
//             type="number"
//             value={hourlyPrice}
//             onChange={(e) => setHourlyPrice(e.target.value)}
//           />
//         </div>
//       </div>

//       <hr />
//       <h3>Uppdatera Personal</h3>
//       <div style={styles.row}>
//         <div style={styles.column}>
//           <label style={styles.label}>Välj från befintlig lista</label>
//           <select
//             style={styles.input}
//             value={selectedStaffId}
//             onChange={handleStaffChange}
//           >
//             <option value="">Välj personal</option>
//             {staff.map((st) => (
//               <option key={st.staffId} value={st.staffId}>
//                 {st.name}
//               </option>
//             ))}
//           </select>
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Namn</label>
//           <input
//             style={styles.input}
//             value={staffName}
//             onChange={(e) => setStaffName(e.target.value)}
//           />
//         </div>

//         <div style={styles.column}>
//           <label style={styles.label}>Roll</label>
//           <input
//             style={styles.input}
//             value={staffRole}
//             onChange={(e) => setStaffRole(e.target.value)}
//           />
//         </div>
//       </div>

//       <div style={styles.buttonRow}>
//         <button style={styles.cancelButton} onClick={onCancelled}>
//           Avbryt
//         </button>
//         <button style={styles.saveButton} onClick={handleUpdate}>
//           Spara
//         </button>
//       </div>
//     </div>
//   );
// }

// const styles = {
//   container: {
//     maxWidth: "800px",
//     margin: "0 auto",
//     padding: "20px",
//     fontFamily: "Arial, sans-serif",
//     backgroundColor: "#f8f8f8",
//   },
//   row: {
//     display: "flex",
//     flexWrap: "wrap",
//     marginBottom: "15px",
//   },
//   column: {
//     flex: "1 1 200px",
//     marginRight: "20px",
//     marginBottom: "10px",
//     minWidth: "200px",
//   },
//   label: { display: "block", marginBottom: "5px", fontWeight: "bold" },
//   input: { width: "100%", padding: "6px", boxSizing: "border-box" },
//   textarea: {
//     width: "100%",
//     padding: "6px",
//     height: "60px",
//     boxSizing: "border-box",
//   },
//   buttonRow: {
//     display: "flex",
//     justifyContent: "flex-end",
//     marginTop: "20px",
//   },
//   cancelButton: {
//     backgroundColor: "#ccc",
//     color: "#000",
//     padding: "8px 16px",
//     border: "none",
//     marginRight: "10px",
//     cursor: "pointer",
//   },
//   saveButton: {
//     backgroundColor: "#4caf50",
//     color: "#fff",
//     padding: "8px 16px",
//     border: "none",
//     cursor: "pointer",
//   },
// };



