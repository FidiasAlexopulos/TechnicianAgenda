import React, { useState, useEffect } from 'react';
import axios from 'axios';

const API_URL = 'https://technicianagenda-production.up.railway.app';

function App() {
    const [clients, setClients] = useState([]);
    const [directions, setDirections] = useState([]);
    const [works, setWorks] = useState([]);
    const [jobCategories, setJobCategories] = useState([]);
    const [jobSubcategories, setJobSubcategories] = useState([]);
    const [regions, setRegions] = useState([]);
    const [comunas, setComunas] = useState([]);
    const [technicians, setTechnicians] = useState([]);
    const [activeTab, setActiveTab] = useState('works');
    const [editingWork, setEditingWork] = useState(null);
    const [editingTechnician, setEditingTechnician] = useState(null);

    // Form states
    const [newClient, setNewClient] = useState({
        name: '',
        apellidos: '',
        telefono: '',
        correoElectronico: ''
    });

    const [newDirection, setNewDirection] = useState({
        clientId: '',
        address: '',
        region: '',
        comuna: '',
        referencia: ''
    });

    const [newWork, setNewWork] = useState({
        jobCategoryId: '',
        jobSubcategoryId: '',
        detalles: '',
        date: '',
        clientId: '',
        directionId: '',
        costos: 0,
        totalACobrar: 0,
        paymentStatus: 0,
        technicianId: '',
        porPagarATecnico: 0,
        pagoATecnicoRealizado: false
    });

    const [newTechnician, setNewTechnician] = useState({
        nombre: '',
        apellidos: '',
        nacionalidad: '',
        rutOPasaporte: '',
        fechaNacimiento: '',
        region: '',
        comuna: '',
        direccion: '',
        correoElectronico: '',
        numeroTelefonico: '',
        numeroTelefonicoAlternativo: '',
        patenteVehiculo: '',
        certificaciones: ''
    });

    const [selectedFiles, setSelectedFiles] = useState([]);
    const [selectedPhoto, setSelectedPhoto] = useState(null);

    const paymentStatusLabels = {
        0: 'Pendiente',
        1: 'Pagado',
        2: 'Pendiente de Pago'
    };

    const paymentStatusColors = {
        0: 'bg-gray-100 text-gray-800',
        1: 'bg-green-100 text-green-800',
        2: 'bg-yellow-100 text-yellow-800'
    };

    // Load initial data
    useEffect(() => {
        loadClients();
        loadWorks();
        loadJobCategories();
        loadRegions();
        loadTechnicians();
    }, []);

    const loadClients = async () => {
        try {
            const response = await axios.get(`${API_URL}/clients`);
            setClients(response.data);
        } catch (error) {
            console.error('Error loading clients:', error);
        }
    };

    const loadDirections = async (clientId) => {
        try {
            const response = await axios.get(`${API_URL}/clients/${clientId}/directions`);
            setDirections(response.data);
        } catch (error) {
            console.error('Error loading directions:', error);
        }
    };

    const loadWorks = async () => {
        try {
            const response = await axios.get(`${API_URL}/works`);
            setWorks(response.data);
        } catch (error) {
            console.error('Error loading works:', error);
        }
    };

    const loadJobCategories = async () => {
        try {
            const response = await axios.get(`${API_URL}/jobcategories`);
            setJobCategories(response.data);
        } catch (error) {
            console.error('Error loading job categories:', error);
        }
    };

    const loadJobSubcategories = async (categoryId) => {
        try {
            const response = await axios.get(`${API_URL}/jobcategories/${categoryId}/subcategories`);
            setJobSubcategories(response.data);
        } catch (error) {
            console.error('Error loading subcategories:', error);
        }
    };

    const loadRegions = async () => {
        try {
            const response = await axios.get(`${API_URL}/regions`);
            setRegions(response.data);
        } catch (error) {
            console.error('Error loading regions:', error);
        }
    };

    const loadComunas = async (regionId) => {
        try {
            const response = await axios.get(`${API_URL}/regions/${regionId}/comunas`);
            setComunas(response.data);
        } catch (error) {
            console.error('Error loading comunas:', error);
        }
    };

    const loadTechnicians = async () => {
        try {
            const response = await axios.get(`${API_URL}/technicians`);
            setTechnicians(response.data);
        } catch (error) {
            console.error('Error loading technicians:', error);
        }
    };

    // Create functions
    const createClient = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_URL}/clients`, newClient);
            setNewClient({ name: '', apellidos: '', telefono: '', correoElectronico: '' });
            loadClients();
            alert('Cliente creado exitosamente!');
        } catch (error) {
            alert('Error al crear cliente');
        }
    };

    const createDirection = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_URL}/directions`, {
                ...newDirection,
                clientId: parseInt(newDirection.clientId),
                region: parseInt(newDirection.region)
            });
            setNewDirection({ clientId: '', address: '', region: '', comuna: '', referencia: '' });
            setComunas([]);
            alert('Dirección agregada!');
            loadClients();
        } catch (error) {
            alert('Error al crear dirección');
        }
    };

    const createWork = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post(`${API_URL}/works`, {
                jobCategoryId: parseInt(newWork.jobCategoryId),
                jobSubcategoryId: parseInt(newWork.jobSubcategoryId),
                detalles: newWork.detalles,
                date: newWork.date,
                status: false,
                clientId: parseInt(newWork.clientId),
                directionId: parseInt(newWork.directionId),
                costos: parseFloat(newWork.costos) || 0,
                totalACobrar: parseFloat(newWork.totalACobrar) || 0,
                paymentStatus: parseInt(newWork.paymentStatus),
                technicianId: newWork.technicianId ? parseInt(newWork.technicianId) : null,
                porPagarATecnico: parseFloat(newWork.porPagarATecnico) || 0,
                pagoATecnicoRealizado: newWork.pagoATecnicoRealizado
            });

            // Upload files if any
            if (selectedFiles.length > 0) {
                await uploadFiles(response.data.id);
            }

            setNewWork({
                jobCategoryId: '',
                jobSubcategoryId: '',
                detalles: '',
                date: '',
                clientId: '',
                directionId: '',
                costos: 0,
                totalACobrar: 0,
                paymentStatus: 0,
                technicianId: '',
                porPagarATecnico: 0,
                pagoATecnicoRealizado: false
            });
            setSelectedFiles([]);
            setJobSubcategories([]);
            setDirections([]);
            loadWorks();
            alert('Orden de trabajo creada!');
        } catch (error) {
            alert('Error al crear orden: ' + (error.response?.data || error.message));
        }
    };

    const createTechnician = async (e) => {
        e.preventDefault();
        try {
            const formData = new FormData();

            // Add all technician data
            Object.keys(newTechnician).forEach(key => {
                if (newTechnician[key]) {
                    formData.append(key, newTechnician[key]);
                }
            });

            // Add photo if selected
            if (selectedPhoto) {
                formData.append('photo', selectedPhoto);
            }

            await axios.post(`${API_URL}/technicians`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            setNewTechnician({
                nombre: '',
                apellidos: '',
                nacionalidad: '',
                rutOPasaporte: '',
                fechaNacimiento: '',
                region: '',
                comuna: '',
                direccion: '',
                correoElectronico: '',
                numeroTelefonico: '',
                numeroTelefonicoAlternativo: '',
                patenteVehiculo: '',
                certificaciones: ''
            });
            setSelectedPhoto(null);
            setComunas([]);
            loadTechnicians();
            alert('Técnico creado exitosamente!');
        } catch (error) {
            alert('Error al crear técnico: ' + (error.response?.data || error.message));
        }
    };

    const updateTechnician = async (e) => {
        e.preventDefault();
        try {
            const formData = new FormData();

            Object.keys(editingTechnician).forEach(key => {
                if (editingTechnician[key] && key !== 'id') {
                    formData.append(key, editingTechnician[key]);
                }
            });

            if (selectedPhoto) {
                formData.append('photo', selectedPhoto);
            }

            await axios.put(`${API_URL}/technicians/${editingTechnician.id}`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });

            setEditingTechnician(null);
            setSelectedPhoto(null);
            loadTechnicians();
            alert('Técnico actualizado!');
        } catch (error) {
            alert('Error al actualizar técnico');
        }
    };

    const uploadFiles = async (workId) => {
        const formData = new FormData();
        selectedFiles.forEach(file => {
            formData.append('files', file);
        });

        try {
            await axios.post(`${API_URL}/works/${workId}/files`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
        } catch (error) {
            console.error('Error uploading files:', error);
        }
    };

    const updateWork = async (e) => {
        e.preventDefault();
        try {
            await axios.put(`${API_URL}/works/${editingWork.id}`, editingWork);

            // Upload new files if any
            if (selectedFiles.length > 0) {
                await uploadFiles(editingWork.id);
            }

            setEditingWork(null);
            setSelectedFiles([]);
            loadWorks();
            alert('Orden actualizada!');
        } catch (error) {
            alert('Error al actualizar orden');
        }
    };

    const toggleWorkStatus = async (workId, currentStatus) => {
        try {
            await axios.patch(`${API_URL}/works/${workId}/status?status=${!currentStatus}`);
            loadWorks();
        } catch (error) {
            alert('Error al actualizar estado');
        }
    };

    const toggleTechnicianPayment = async (workId, currentStatus) => {
        try {
            await axios.patch(`${API_URL}/works/${workId}/technician-payment?paid=${!currentStatus}`);
            loadWorks();
        } catch (error) {
            alert('Error al actualizar pago a técnico');
        }
    };

    const deleteFile = async (fileId) => {
        if (!window.confirm('¿Eliminar este archivo?')) return;

        try {
            await axios.delete(`${API_URL}/files/${fileId}`);
            loadWorks();
            if (editingWork) {
                const updatedWork = await axios.get(`${API_URL}/works/${editingWork.id}`);
                setEditingWork(updatedWork.data);
            }
        } catch (error) {
            alert('Error al eliminar archivo');
        }
    };

    const handleCategoryChange = (categoryId, isEdit = false) => {
        if (isEdit) {
            setEditingWork({ ...editingWork, jobCategoryId: categoryId, jobSubcategoryId: '' });
        } else {
            setNewWork({ ...newWork, jobCategoryId: categoryId, jobSubcategoryId: '' });
        }

        if (categoryId) {
            loadJobSubcategories(categoryId);
        } else {
            setJobSubcategories([]);
        }
    };

    const handleClientChangeForWork = (clientId, isEdit = false) => {
        if (isEdit) {
            setEditingWork({ ...editingWork, clientId, directionId: '' });
        } else {
            setNewWork({ ...newWork, clientId, directionId: '' });
        }

        if (clientId) {
            loadDirections(clientId);
        } else {
            setDirections([]);
        }
    };

    const handleRegionChange = (regionId, target = 'direction') => {
        if (target === 'direction') {
            setNewDirection({ ...newDirection, region: regionId, comuna: '' });
        } else if (target === 'technician') {
            setNewTechnician({ ...newTechnician, region: regionId, comuna: '' });
        } else if (target === 'editTechnician') {
            setEditingTechnician({ ...editingTechnician, region: regionId, comuna: '' });
        }

        if (regionId) {
            loadComunas(regionId);
        } else {
            setComunas([]);
        }
    };

    const handleFileSelect = (e) => {
        setSelectedFiles(Array.from(e.target.files));
    };

    const handlePhotoSelect = (e) => {
        if (e.target.files && e.target.files[0]) {
            setSelectedPhoto(e.target.files[0]);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50">
            <header className="bg-blue-600 text-white shadow-lg">
                <div className="container mx-auto px-4 py-6">
                    <h1 className="text-2xl md:text-3xl font-bold">🔧 Agenda de Técnicos</h1>
                    <p className="text-blue-100 text-sm mt-1">Gestiona tus órdenes de trabajo y técnicos</p>
                </div>
            </header>

            <div className="bg-white shadow">
                <div className="container mx-auto px-4">
                    <div className="flex space-x-1 overflow-x-auto">
                        {['works', 'technicians', 'clients', 'addresses'].map((tab) => (
                            <button
                                key={tab}
                                onClick={() => setActiveTab(tab)}
                                className={`px-6 py-3 font-medium capitalize whitespace-nowrap ${activeTab === tab
                                        ? 'text-blue-600 border-b-2 border-blue-600'
                                        : 'text-gray-600 hover:text-blue-600'
                                    }`}
                            >
                                {tab === 'works' ? 'Trabajos' :
                                    tab === 'technicians' ? 'Técnicos' :
                                        tab === 'clients' ? 'Clientes' : 'Direcciones'}
                            </button>
                        ))}
                    </div>
                </div>
            </div>

            <div className="container mx-auto px-4 py-6">
                {/* WORK ORDERS TAB */}
                {activeTab === 'works' && (
                    <div className="space-y-6">
                        {/* Create/Edit Work Order Form */}
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">
                                {editingWork ? 'Editar Orden de Trabajo' : 'Crear Orden de Trabajo'}
                            </h2>
                            <form onSubmit={editingWork ? updateWork : createWork} className="space-y-4">
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Categoría *
                                        </label>
                                        <select
                                            value={editingWork ? editingWork.jobCategoryId : newWork.jobCategoryId}
                                            onChange={(e) => handleCategoryChange(e.target.value, !!editingWork)}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="">Seleccionar Categoría</option>
                                            {jobCategories.map((cat) => (
                                                <option key={cat.id} value={cat.id}>
                                                    {cat.icon} {cat.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Tipo de Trabajo *
                                        </label>
                                        <select
                                            value={editingWork ? editingWork.jobSubcategoryId : newWork.jobSubcategoryId}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, jobSubcategoryId: e.target.value })
                                                    : setNewWork({ ...newWork, jobSubcategoryId: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                            disabled={!(editingWork ? editingWork.jobCategoryId : newWork.jobCategoryId)}
                                        >
                                            <option value="">Seleccionar Tipo</option>
                                            {jobSubcategories.map((sub) => (
                                                <option key={sub.id} value={sub.id}>
                                                    {sub.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Cliente *
                                        </label>
                                        <select
                                            value={editingWork ? editingWork.clientId : newWork.clientId}
                                            onChange={(e) => handleClientChangeForWork(e.target.value, !!editingWork)}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="">Seleccionar Cliente</option>
                                            {clients.map((client) => (
                                                <option key={client.id} value={client.id}>
                                                    {client.name} {client.apellidos}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Dirección *
                                        </label>
                                        <select
                                            value={editingWork ? editingWork.directionId : newWork.directionId}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, directionId: e.target.value })
                                                    : setNewWork({ ...newWork, directionId: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                            disabled={!(editingWork ? editingWork.clientId : newWork.clientId)}
                                        >
                                            <option value="">Seleccionar Dirección</option>
                                            {directions.map((dir) => (
                                                <option key={dir.id} value={dir.id}>
                                                    {dir.address}, {dir.comuna}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Técnico Asignado
                                        </label>
                                        <select
                                            value={editingWork ? editingWork.technicianId || '' : newWork.technicianId}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, technicianId: e.target.value })
                                                    : setNewWork({ ...newWork, technicianId: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        >
                                            <option value="">Sin asignar</option>
                                            {technicians.map((tech) => (
                                                <option key={tech.id} value={tech.id}>
                                                    {tech.nombre} {tech.apellidos} - {tech.rutOPasaporte}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Fecha y Hora *
                                        </label>
                                        <input
                                            type="datetime-local"
                                            value={editingWork ? editingWork.date : newWork.date}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, date: e.target.value })
                                                    : setNewWork({ ...newWork, date: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Estado de Pago *
                                        </label>
                                        <select
                                            value={editingWork ? editingWork.paymentStatus : newWork.paymentStatus}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, paymentStatus: parseInt(e.target.value) })
                                                    : setNewWork({ ...newWork, paymentStatus: parseInt(e.target.value) })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="0">Pendiente</option>
                                            <option value="1">Pagado</option>
                                            <option value="2">Pendiente de Pago</option>
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Costos ($)
                                        </label>
                                        <input
                                            type="number"
                                            step="0.01"
                                            value={editingWork ? editingWork.costos : newWork.costos}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, costos: e.target.value })
                                                    : setNewWork({ ...newWork, costos: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            placeholder="0.00"
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Total a Cobrar ($)
                                        </label>
                                        <input
                                            type="number"
                                            step="0.01"
                                            value={editingWork ? editingWork.totalACobrar : newWork.totalACobrar}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, totalACobrar: e.target.value })
                                                    : setNewWork({ ...newWork, totalACobrar: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            placeholder="0.00"
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Por Pagar a Técnico ($)
                                        </label>
                                        <input
                                            type="number"
                                            step="0.01"
                                            value={editingWork ? editingWork.porPagarATecnico : newWork.porPagarATecnico}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, porPagarATecnico: e.target.value })
                                                    : setNewWork({ ...newWork, porPagarATecnico: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            placeholder="0.00"
                                        />
                                    </div>

                                    <div className="md:col-span-2">
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Detalles
                                        </label>
                                        <textarea
                                            value={editingWork ? editingWork.detalles : newWork.detalles}
                                            onChange={(e) =>
                                                editingWork
                                                    ? setEditingWork({ ...editingWork, detalles: e.target.value })
                                                    : setNewWork({ ...newWork, detalles: e.target.value })
                                            }
                                            placeholder="Descripción del trabajo..."
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            rows="3"
                                        />
                                    </div>

                                    <div className="md:col-span-2">
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Imágenes/Videos
                                        </label>
                                        <input
                                            type="file"
                                            multiple
                                            accept="image/*,video/*"
                                            onChange={handleFileSelect}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        />
                                        {selectedFiles.length > 0 && (
                                            <p className="text-sm text-gray-600 mt-2">
                                                {selectedFiles.length} archivo(s) seleccionado(s)
                                            </p>
                                        )}
                                    </div>

                                    {editingWork && editingWork.files && editingWork.files.length > 0 && (
                                        <div className="md:col-span-2">
                                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                                Archivos Existentes
                                            </label>
                                            <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
                                                {editingWork.files.map((file) => (
                                                    <div key={file.id} className="relative">
                                                        {file.fileType === 'image' ? (
                                                            <img
                                                                src={`${API_URL}${file.filePath}`}
                                                                alt={file.fileName}
                                                                className="w-full h-24 object-cover rounded"
                                                            />
                                                        ) : (
                                                            <video
                                                                src={`${API_URL}${file.filePath}`}
                                                                className="w-full h-24 object-cover rounded"
                                                                controls
                                                            />
                                                        )}
                                                        <button
                                                            type="button"
                                                            onClick={() => deleteFile(file.id)}
                                                            className="absolute top-1 right-1 bg-red-500 text-white rounded-full w-6 h-6 flex items-center justify-center text-xs hover:bg-red-600"
                                                        >
                                                            ✕
                                                        </button>
                                                    </div>
                                                ))}
                                            </div>
                                        </div>
                                    )}
                                </div>

                                <div className="flex gap-2">
                                    <button
                                        type="submit"
                                        className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    >
                                        {editingWork ? 'Actualizar Orden' : 'Crear Orden'}
                                    </button>
                                    {editingWork && (
                                        <button
                                            type="button"
                                            onClick={() => {
                                                setEditingWork(null);
                                                setSelectedFiles([]);
                                            }}
                                            className="px-6 py-2 bg-gray-300 text-gray-700 rounded-md hover:bg-gray-400"
                                        >
                                            Cancelar
                                        </button>
                                    )}
                                </div>
                            </form>
                        </div>

                        {/* Work Orders List */}
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Órdenes de Trabajo</h2>
                            <div className="space-y-3">
                                {works.length === 0 ? (
                                    <p className="text-gray-500 text-center py-8">No hay órdenes aún</p>
                                ) : (
                                    works.map((work) => (
                                        <div
                                            key={work.id}
                                            className={`border rounded-lg p-4 ${work.status ? 'bg-green-50 border-green-200' : 'bg-white border-gray-200'
                                                }`}
                                        >
                                            <div className="flex items-start justify-between">
                                                <div className="flex-1">
                                                    <div className="flex items-center space-x-2 mb-2 flex-wrap">
                                                        <span className="text-2xl">{work.jobCategory?.icon}</span>
                                                        <div>
                                                            <span className="font-semibold text-lg">
                                                                {work.jobCategory?.name}
                                                            </span>
                                                            <p className="text-sm text-gray-600">
                                                                {work.jobSubcategory?.name}
                                                            </p>
                                                        </div>
                                                        {work.status && (
                                                            <span className="px-2 py-1 bg-green-500 text-white text-xs rounded-full">
                                                                ✓ Completado
                                                            </span>
                                                        )}
                                                        <span
                                                            className={`px-2 py-1 text-xs rounded-full ${paymentStatusColors[work.paymentStatus]
                                                                }`}
                                                        >
                                                            💰 {paymentStatusLabels[work.paymentStatus]}
                                                        </span>
                                                    </div>

                                                    <div className="mt-3 space-y-1">
                                                        <p className="text-gray-700">
                                                            <strong>Cliente:</strong> {work.client?.name} {work.client?.apellidos}
                                                        </p>
                                                        <p className="text-gray-700">
                                                            <strong>Dirección:</strong> {work.direction?.address}
                                                            {work.direction?.comuna && `, ${work.direction.comuna}`}
                                                        </p>
                                                        {work.technician && (
                                                            <p className="text-gray-700">
                                                                <strong>Técnico:</strong> {work.technician.nombre} {work.technician.apellidos} ({work.technician.rutOPasaporte})
                                                            </p>
                                                        )}
                                                        <p className="text-gray-600 text-sm">
                                                            <strong>Fecha:</strong>{' '}
                                                            {new Date(work.date).toLocaleString('es-CL')}
                                                        </p>
                                                        {work.detalles && (
                                                            <p className="text-gray-600 text-sm">
                                                                <strong>Detalles:</strong> {work.detalles}
                                                            </p>
                                                        )}
                                                        <div className="flex gap-4 mt-2 flex-wrap">
                                                            <p className="text-gray-700 text-sm">
                                                                <strong>Costos:</strong> ${work.costos.toLocaleString('es-CL')}
                                                            </p>
                                                            <p className="text-gray-700 text-sm">
                                                                <strong>Total a Cobrar:</strong> ${work.totalACobrar.toLocaleString('es-CL')}
                                                            </p>
                                                            {work.porPagarATecnico > 0 && (
                                                                <p className="text-gray-700 text-sm">
                                                                    <strong>Por Pagar a Técnico:</strong> ${work.porPagarATecnico.toLocaleString('es-CL')}
                                                                    <span className={`ml-2 px-2 py-1 text-xs rounded-full ${work.pagoATecnicoRealizado
                                                                            ? 'bg-green-100 text-green-800'
                                                                            : 'bg-red-100 text-red-800'
                                                                        }`}>
                                                                        {work.pagoATecnicoRealizado ? '✓ Pagado' : '✗ Pendiente'}
                                                                    </span>
                                                                </p>
                                                            )}
                                                        </div>
                                                        {work.files && work.files.length > 0 && (
                                                            <div className="mt-2">
                                                                <p className="text-sm font-medium text-gray-700 mb-2">
                                                                    Archivos ({work.files.length})
                                                                </p>
                                                                <div className="flex gap-2 overflow-x-auto">
                                                                    {work.files.map((file) => (
                                                                        <div key={file.id} className="flex-shrink-0">
                                                                            {file.fileType === 'image' ? (
                                                                                <img
                                                                                    src={`${API_URL}${file.filePath}`}
                                                                                    alt={file.fileName}
                                                                                    className="w-20 h-20 object-cover rounded cursor-pointer hover:opacity-80"
                                                                                    onClick={() => window.open(`${API_URL}${file.filePath}`, '_blank')}
                                                                                />
                                                                            ) : (
                                                                                <video
                                                                                    src={`${API_URL}${file.filePath}`}
                                                                                    className="w-20 h-20 object-cover rounded"
                                                                                    controls
                                                                                />
                                                                            )}
                                                                        </div>
                                                                    ))}
                                                                </div>
                                                            </div>
                                                        )}
                                                    </div>
                                                </div>

                                                <div className="ml-4 flex flex-col gap-2">
                                                    <button
                                                        onClick={() => toggleWorkStatus(work.id, work.status)}
                                                        className={`px-4 py-2 rounded-md text-sm font-medium whitespace-nowrap ${work.status
                                                                ? 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                                                                : 'bg-green-500 text-white hover:bg-green-600'
                                                            }`}
                                                    >
                                                        {work.status ? 'Marcar Pendiente' : 'Completar'}
                                                    </button>
                                                    {work.porPagarATecnico > 0 && (
                                                        <button
                                                            onClick={() => toggleTechnicianPayment(work.id, work.pagoATecnicoRealizado)}
                                                            className={`px-4 py-2 rounded-md text-sm font-medium whitespace-nowrap ${work.pagoATecnicoRealizado
                                                                    ? 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                                                                    : 'bg-yellow-500 text-white hover:bg-yellow-600'
                                                                }`}
                                                        >
                                                            {work.pagoATecnicoRealizado ? 'Marcar No Pagado' : 'Marcar Pagado'}
                                                        </button>
                                                    )}
                                                    <button
                                                        onClick={() => {
                                                            setEditingWork({
                                                                ...work,
                                                                date: work.date.substring(0, 16)
                                                            });
                                                            if (work.jobCategoryId) {
                                                                loadJobSubcategories(work.jobCategoryId);
                                                            }
                                                            if (work.clientId) {
                                                                loadDirections(work.clientId);
                                                            }
                                                            window.scrollTo({ top: 0, behavior: 'smooth' });
                                                        }}
                                                        className="px-4 py-2 bg-blue-500 text-white rounded-md text-sm font-medium hover:bg-blue-600"
                                                    >
                                                        Editar
                                                    </button>
                                                </div>
                                            </div>
                                        </div>
                                    ))
                                )}
                            </div>
                        </div>
                    </div>
                )}

                {/* TECHNICIANS TAB */}
                {activeTab === 'technicians' && (
                    <div className="space-y-6">
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">
                                {editingTechnician ? 'Editar Técnico' : 'Agregar Nuevo Técnico'}
                            </h2>
                            <form onSubmit={editingTechnician ? updateTechnician : createTechnician} className="space-y-4">
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Nombre *
                                        </label>
                                        <input
                                            type="text"
                                            value={editingTechnician ? editingTechnician.nombre : newTechnician.nombre}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, nombre: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, nombre: e.target.value })
                                            }
                                            placeholder="Juan"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Apellidos *
                                        </label>
                                        <input
                                            type="text"
                                            value={editingTechnician ? editingTechnician.apellidos : newTechnician.apellidos}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, apellidos: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, apellidos: e.target.value })
                                            }
                                            placeholder="Pérez González"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Nacionalidad *
                                        </label>
                                        <input
                                            type="text"
                                            value={editingTechnician ? editingTechnician.nacionalidad : newTechnician.nacionalidad}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, nacionalidad: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, nacionalidad: e.target.value })
                                            }
                                            placeholder="Chilena"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            RUT o Pasaporte *
                                        </label>
                                        <input
                                            type="text"
                                            value={editingTechnician ? editingTechnician.rutOPasaporte : newTechnician.rutOPasaporte}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, rutOPasaporte: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, rutOPasaporte: e.target.value })
                                            }
                                            placeholder="12.345.678-9"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Fecha de Nacimiento *
                                        </label>
                                        <input
                                            type="date"
                                            value={editingTechnician ? editingTechnician.fechaNacimiento : newTechnician.fechaNacimiento}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, fechaNacimiento: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, fechaNacimiento: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Fotografía
                                        </label>
                                        <input
                                            type="file"
                                            accept="image/*"
                                            onChange={handlePhotoSelect}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        />
                                        {selectedPhoto && (
                                            <p className="text-sm text-gray-600 mt-2">
                                                Archivo seleccionado: {selectedPhoto.name}
                                            </p>
                                        )}
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Región *
                                        </label>
                                        <select
                                            value={editingTechnician ? editingTechnician.region : newTechnician.region}
                                            onChange={(e) => handleRegionChange(e.target.value, editingTechnician ? 'editTechnician' : 'technician')}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="">Seleccionar Región</option>
                                            {regions.map((region) => (
                                                <option key={region.id} value={region.id}>
                                                    {region.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Comuna *
                                        </label>
                                        <select
                                            value={editingTechnician ? editingTechnician.comuna : newTechnician.comuna}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, comuna: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, comuna: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                            disabled={!(editingTechnician ? editingTechnician.region : newTechnician.region)}
                                        >
                                            <option value="">Seleccionar Comuna</option>
                                            {comunas.map((comuna, index) => (
                                                <option key={index} value={comuna}>
                                                    {comuna}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div className="md:col-span-2">
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Dirección *
                                        </label>
                                        <input
                                            type="text"
                                            value={editingTechnician ? editingTechnician.direccion : newTechnician.direccion}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, direccion: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, direccion: e.target.value })
                                            }
                                            placeholder="Av. Libertador Bernardo O'Higgins 123"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Correo Electrónico *
                                        </label>
                                        <input
                                            type="email"
                                            value={editingTechnician ? editingTechnician.correoElectronico : newTechnician.correoElectronico}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, correoElectronico: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, correoElectronico: e.target.value })
                                            }
                                            placeholder="juan@example.com"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Número Telefónico *
                                        </label>
                                        <input
                                            type="tel"
                                            value={editingTechnician ? editingTechnician.numeroTelefonico : newTechnician.numeroTelefonico}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, numeroTelefonico: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, numeroTelefonico: e.target.value })
                                            }
                                            placeholder="+56 9 1234 5678"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Número Telefónico Alternativo
                                        </label>
                                        <input
                                            type="tel"
                                            value={editingTechnician ? editingTechnician.numeroTelefonicoAlternativo : newTechnician.numeroTelefonicoAlternativo}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, numeroTelefonicoAlternativo: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, numeroTelefonicoAlternativo: e.target.value })
                                            }
                                            placeholder="+56 9 8765 4321"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Patente Vehículo
                                        </label>
                                        <input
                                            type="text"
                                            value={editingTechnician ? editingTechnician.patenteVehiculo : newTechnician.patenteVehiculo}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, patenteVehiculo: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, patenteVehiculo: e.target.value })
                                            }
                                            placeholder="ABCD-12"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        />
                                    </div>

                                    <div className="md:col-span-2">
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Certificaciones
                                        </label>
                                        <textarea
                                            value={editingTechnician ? editingTechnician.certificaciones : newTechnician.certificaciones}
                                            onChange={(e) =>
                                                editingTechnician
                                                    ? setEditingTechnician({ ...editingTechnician, certificaciones: e.target.value })
                                                    : setNewTechnician({ ...newTechnician, certificaciones: e.target.value })
                                            }
                                            placeholder="Ej: Certificación en instalación eléctrica, Curso de refrigeración..."
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            rows="3"
                                        />
                                    </div>
                                </div>

                                <div className="flex gap-2">
                                    <button
                                        type="submit"
                                        className="w-full md:w-auto px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    >
                                        {editingTechnician ? 'Actualizar Técnico' : 'Agregar Técnico'}
                                    </button>
                                    {editingTechnician && (
                                        <button
                                            type="button"
                                            onClick={() => {
                                                setEditingTechnician(null);
                                                setSelectedPhoto(null);
                                            }}
                                            className="px-6 py-2 bg-gray-300 text-gray-700 rounded-md hover:bg-gray-400"
                                        >
                                            Cancelar
                                        </button>
                                    )}
                                </div>
                            </form>
                        </div>

                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Lista de Técnicos</h2>
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                                {technicians.map((tech) => (
                                    <div key={tech.id} className="border border-gray-200 rounded-lg p-4">
                                        {tech.fotografiaPath && (
                                            <img
                                                src={`${API_URL}${tech.fotografiaPath}`}
                                                alt={`${tech.nombre} ${tech.apellidos}`}
                                                className="w-20 h-20 rounded-full object-cover mx-auto mb-3"
                                            />
                                        )}
                                        <h3 className="font-semibold text-lg text-center">
                                            {tech.nombre} {tech.apellidos}
                                        </h3>
                                        <p className="text-sm text-gray-600 text-center">
                                            {tech.rutOPasaporte}
                                        </p>
                                        <div className="mt-3 space-y-1">
                                            <p className="text-sm text-gray-600">
                                                📧 {tech.correoElectronico}
                                            </p>
                                            <p className="text-sm text-gray-600">
                                                📱 {tech.numeroTelefonico}
                                            </p>
                                            {tech.patenteVehiculo && (
                                                <p className="text-sm text-gray-600">
                                                    🚗 {tech.patenteVehiculo}
                                                </p>
                                            )}
                                            <p className="text-sm text-gray-600">
                                                📍 {tech.comuna}
                                            </p>
                                        </div>
                                        <button
                                            onClick={() => {
                                                setEditingTechnician(tech);
                                                if (tech.region) {
                                                    loadComunas(tech.region);
                                                }
                                                window.scrollTo({ top: 0, behavior: 'smooth' });
                                            }}
                                            className="w-full mt-3 px-4 py-2 bg-blue-500 text-white rounded-md text-sm font-medium hover:bg-blue-600"
                                        >
                                            Editar
                                        </button>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                )}

                {/* CLIENTS TAB */}
                {activeTab === 'clients' && (
                    <div className="space-y-6">
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Agregar Nuevo Cliente</h2>
                            <form onSubmit={createClient} className="space-y-4">
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Nombre *
                                        </label>
                                        <input
                                            type="text"
                                            value={newClient.name}
                                            onChange={(e) => setNewClient({ ...newClient, name: e.target.value })}
                                            placeholder="Juan"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Apellidos *
                                        </label>
                                        <input
                                            type="text"
                                            value={newClient.apellidos}
                                            onChange={(e) => setNewClient({ ...newClient, apellidos: e.target.value })}
                                            placeholder="Pérez González"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Teléfono
                                        </label>
                                        <input
                                            type="tel"
                                            value={newClient.telefono}
                                            onChange={(e) => setNewClient({ ...newClient, telefono: e.target.value })}
                                            placeholder="+56 9 1234 5678"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Correo Electrónico
                                        </label>
                                        <input
                                            type="email"
                                            value={newClient.correoElectronico}
                                            onChange={(e) =>
                                                setNewClient({ ...newClient, correoElectronico: e.target.value })
                                            }
                                            placeholder="juan@example.com"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        />
                                    </div>
                                </div>

                                <button
                                    type="submit"
                                    className="w-full md:w-auto px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    Agregar Cliente
                                </button>
                            </form>
                        </div>

                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Lista de Clientes</h2>
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                                {clients.map((client) => (
                                    <div key={client.id} className="border border-gray-200 rounded-lg p-4">
                                        <h3 className="font-semibold text-lg">
                                            {client.name} {client.apellidos}
                                        </h3>
                                        {client.telefono && (
                                            <p className="text-sm text-gray-600 mt-1">📱 {client.telefono}</p>
                                        )}
                                        {client.correoElectronico && (
                                            <p className="text-sm text-gray-600">✉️ {client.correoElectronico}</p>
                                        )}
                                        <p className="text-sm text-gray-600 mt-2">
                                            📍 {client.directions?.length || 0} dirección(es)
                                        </p>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                )}

                {/* ADDRESSES TAB */}
                {activeTab === 'addresses' && (
                    <div className="space-y-6">
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Agregar Nueva Dirección</h2>
                            <form onSubmit={createDirection} className="space-y-4">
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div className="md:col-span-2">
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Cliente *
                                        </label>
                                        <select
                                            value={newDirection.clientId}
                                            onChange={(e) =>
                                                setNewDirection({ ...newDirection, clientId: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="">Seleccionar Cliente</option>
                                            {clients.map((client) => (
                                                <option key={client.id} value={client.id}>
                                                    {client.name} {client.apellidos}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div className="md:col-span-2">
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Dirección *
                                        </label>
                                        <input
                                            type="text"
                                            value={newDirection.address}
                                            onChange={(e) =>
                                                setNewDirection({ ...newDirection, address: e.target.value })
                                            }
                                            placeholder="Av. Libertador Bernardo O'Higgins 123"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Región *
                                        </label>
                                        <select
                                            value={newDirection.region}
                                            onChange={(e) => handleRegionChange(e.target.value, 'direction')}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="">Seleccionar Región</option>
                                            {regions.map((region) => (
                                                <option key={region.id} value={region.id}>
                                                    {region.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Comuna *
                                        </label>
                                        <select
                                            value={newDirection.comuna}
                                            onChange={(e) =>
                                                setNewDirection({ ...newDirection, comuna: e.target.value })
                                            }
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                            disabled={!newDirection.region}
                                        >
                                            <option value="">Seleccionar Comuna</option>
                                            {comunas.map((comuna, index) => (
                                                <option key={index} value={comuna}>
                                                    {comuna}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div className="md:col-span-2">
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Referencia
                                        </label>
                                        <input
                                            type="text"
                                            value={newDirection.referencia}
                                            onChange={(e) =>
                                                setNewDirection({ ...newDirection, referencia: e.target.value })
                                            }
                                            placeholder="Edificio azul, depto 302"
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        />
                                    </div>
                                </div>

                                <button
                                    type="submit"
                                    className="w-full md:w-auto px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    Agregar Dirección
                                </button>
                            </form>
                        </div>

                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Todas las Direcciones</h2>
                            <div className="space-y-4">
                                {clients.map((client) => (
                                    <div key={client.id}>
                                        <h3 className="font-semibold text-lg mb-2">
                                            {client.name} {client.apellidos}
                                        </h3>
                                        {client.directions && client.directions.length > 0 ? (
                                            <div className="ml-4 space-y-2">
                                                {client.directions.map((dir) => (
                                                    <div
                                                        key={dir.id}
                                                        className="border-l-4 border-blue-500 pl-4 py-2 bg-gray-50"
                                                    >
                                                        <p className="font-medium">📍 {dir.address}</p>
                                                        {dir.comuna && (
                                                            <p className="text-sm text-gray-600">Comuna: {dir.comuna}</p>
                                                        )}
                                                        {dir.referencia && (
                                                            <p className="text-sm text-gray-500 italic">
                                                                Ref: {dir.referencia}
                                                            </p>
                                                        )}
                                                    </div>
                                                ))}
                                            </div>
                                        ) : (
                                            <p className="text-gray-500 ml-4 text-sm">Sin direcciones aún</p>
                                        )}
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

export default App;