import React, { useState, useEffect } from 'react';
import axios from 'axios';

const API_URL = 'https://localhost:7054/api';

function App() {
    const [clients, setClients] = useState([]);
    const [directions, setDirections] = useState([]);
    const [works, setWorks] = useState([]);
    const [selectedClient, setSelectedClient] = useState(null);
    const [activeTab, setActiveTab] = useState('works');

    // Form states
    const [newClient, setNewClient] = useState('');
    const [newDirection, setNewDirection] = useState({ clientId: '', address: '' });
    const [newWork, setNewWork] = useState({
        jobType: 'plumber',
        date: '',
        clientId: '',
        directionId: ''
    });

    // Load data
    useEffect(() => {
        loadClients();
        loadWorks();
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

    // Create functions
    const createClient = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_URL}/clients`, { name: newClient });
            setNewClient('');
            loadClients();
            alert('Client created!');
        } catch (error) {
            alert('Error creating client');
        }
    };

    const createDirection = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_URL}/directions`, newDirection);
            setNewDirection({ clientId: '', address: '' });
            alert('Address added!');
            if (newDirection.clientId) {
                loadDirections(newDirection.clientId);
            }
        } catch (error) {
            alert('Error creating direction');
        }
    };

    const createWork = async (e) => {
        e.preventDefault();
        try {
            await axios.post(`${API_URL}/works`, {
                ...newWork,
                status: false,
                clientId: parseInt(newWork.clientId),
                directionId: parseInt(newWork.directionId)
            });
            setNewWork({ jobType: 'plumber', date: '', clientId: '', directionId: '' });
            loadWorks();
            alert('Work order created!');
        } catch (error) {
            alert('Error creating work order: ' + (error.response?.data || error.message));
        }
    };

    const toggleWorkStatus = async (workId, currentStatus) => {
        try {
            await axios.patch(`${API_URL}/works/${workId}/status?status=${!currentStatus}`);
            loadWorks();
        } catch (error) {
            alert('Error updating status');
        }
    };

    const handleClientChange = (clientId) => {
        setSelectedClient(clientId);
        if (clientId) {
            loadDirections(clientId);
        } else {
            setDirections([]);
        }
    };

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Header */}
            <header className="bg-blue-600 text-white shadow-lg">
                <div className="container mx-auto px-4 py-6">
                    <h1 className="text-2xl md:text-3xl font-bold">🔧 Technician Agenda</h1>
                    <p className="text-blue-100 text-sm mt-1">Manage your work orders</p>
                </div>
            </header>

            {/* Navigation Tabs */}
            <div className="bg-white shadow">
                <div className="container mx-auto px-4">
                    <div className="flex space-x-1 overflow-x-auto">
                        {['works', 'clients', 'addresses'].map((tab) => (
                            <button
                                key={tab}
                                onClick={() => setActiveTab(tab)}
                                className={`px-6 py-3 font-medium capitalize whitespace-nowrap ${activeTab === tab
                                        ? 'text-blue-600 border-b-2 border-blue-600'
                                        : 'text-gray-600 hover:text-blue-600'
                                    }`}
                            >
                                {tab}
                            </button>
                        ))}
                    </div>
                </div>
            </div>

            <div className="container mx-auto px-4 py-6">
                {/* Work Orders Tab */}
                {activeTab === 'works' && (
                    <div className="space-y-6">
                        {/* Create Work Order Form */}
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Create Work Order</h2>
                            <form onSubmit={createWork} className="space-y-4">
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Job Type
                                        </label>
                                        <select
                                            value={newWork.jobType}
                                            onChange={(e) => setNewWork({ ...newWork, jobType: e.target.value })}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="plumber">🚰 Plumber</option>
                                            <option value="electricity">⚡ Electrician</option>
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Date
                                        </label>
                                        <input
                                            type="datetime-local"
                                            value={newWork.date}
                                            onChange={(e) => setNewWork({ ...newWork, date: e.target.value })}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        />
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Client
                                        </label>
                                        <select
                                            value={newWork.clientId}
                                            onChange={(e) => {
                                                const clientId = e.target.value;
                                                setNewWork({ ...newWork, clientId, directionId: '' });
                                                handleClientChange(clientId);
                                            }}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                        >
                                            <option value="">Select Client</option>
                                            {clients.map((client) => (
                                                <option key={client.id} value={client.id}>
                                                    {client.name}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    <div>
                                        <label className="block text-sm font-medium text-gray-700 mb-2">
                                            Address
                                        </label>
                                        <select
                                            value={newWork.directionId}
                                            onChange={(e) => setNewWork({ ...newWork, directionId: e.target.value })}
                                            className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                            required
                                            disabled={!newWork.clientId}
                                        >
                                            <option value="">Select Address</option>
                                            {directions.map((dir) => (
                                                <option key={dir.id} value={dir.id}>
                                                    {dir.address}
                                                </option>
                                            ))}
                                        </select>
                                    </div>
                                </div>

                                <button
                                    type="submit"
                                    className="w-full md:w-auto px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    Create Work Order
                                </button>
                            </form>
                        </div>

                        {/* Work Orders List */}
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Work Orders</h2>
                            <div className="space-y-3">
                                {works.length === 0 ? (
                                    <p className="text-gray-500 text-center py-8">No work orders yet</p>
                                ) : (
                                    works.map((work) => (
                                        <div
                                            key={work.id}
                                            className={`border rounded-lg p-4 ${work.status ? 'bg-green-50 border-green-200' : 'bg-white border-gray-200'
                                                }`}
                                        >
                                            <div className="flex items-start justify-between">
                                                <div className="flex-1">
                                                    <div className="flex items-center space-x-2 mb-2">
                                                        <span className="text-2xl">
                                                            {work.jobType === 'plumber' ? '🚰' : '⚡'}
                                                        </span>
                                                        <span className="font-semibold text-lg capitalize">
                                                            {work.jobType}
                                                        </span>
                                                        {work.status && (
                                                            <span className="px-2 py-1 bg-green-500 text-white text-xs rounded-full">
                                                                ✓ Done
                                                            </span>
                                                        )}
                                                    </div>
                                                    <p className="text-gray-700">
                                                        <strong>Client:</strong> {work.client?.name}
                                                    </p>
                                                    <p className="text-gray-700">
                                                        <strong>Address:</strong> {work.direction?.address}
                                                    </p>
                                                    <p className="text-gray-600 text-sm">
                                                        <strong>Date:</strong>{' '}
                                                        {new Date(work.date).toLocaleString()}
                                                    </p>
                                                </div>
                                                <button
                                                    onClick={() => toggleWorkStatus(work.id, work.status)}
                                                    className={`px-4 py-2 rounded-md text-sm font-medium ${work.status
                                                            ? 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                                                            : 'bg-green-500 text-white hover:bg-green-600'
                                                        }`}
                                                >
                                                    {work.status ? 'Mark Undone' : 'Mark Done'}
                                                </button>
                                            </div>
                                        </div>
                                    ))
                                )}
                            </div>
                        </div>
                    </div>
                )}

                {/* Clients Tab */}
                {activeTab === 'clients' && (
                    <div className="space-y-6">
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Add New Client</h2>
                            <form onSubmit={createClient} className="flex gap-2">
                                <input
                                    type="text"
                                    value={newClient}
                                    onChange={(e) => setNewClient(e.target.value)}
                                    placeholder="Client name"
                                    className="flex-1 px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                    required
                                />
                                <button
                                    type="submit"
                                    className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    Add
                                </button>
                            </form>
                        </div>

                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Clients List</h2>
                            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-3">
                                {clients.map((client) => (
                                    <div key={client.id} className="border border-gray-200 rounded-lg p-4">
                                        <h3 className="font-semibold text-lg">{client.name}</h3>
                                        <p className="text-sm text-gray-600 mt-1">
                                            {client.directions?.length || 0} address(es)
                                        </p>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>
                )}

                {/* Addresses Tab */}
                {activeTab === 'addresses' && (
                    <div className="space-y-6">
                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">Add New Address</h2>
                            <form onSubmit={createDirection} className="space-y-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Client
                                    </label>
                                    <select
                                        value={newDirection.clientId}
                                        onChange={(e) =>
                                            setNewDirection({ ...newDirection, clientId: e.target.value })
                                        }
                                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        required
                                    >
                                        <option value="">Select Client</option>
                                        {clients.map((client) => (
                                            <option key={client.id} value={client.id}>
                                                {client.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div>
                                    <label className="block text-sm font-medium text-gray-700 mb-2">
                                        Address
                                    </label>
                                    <input
                                        type="text"
                                        value={newDirection.address}
                                        onChange={(e) =>
                                            setNewDirection({ ...newDirection, address: e.target.value })
                                        }
                                        placeholder="123 Main Street, Santiago"
                                        className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                                        required
                                    />
                                </div>
                                <button
                                    type="submit"
                                    className="w-full md:w-auto px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
                                >
                                    Add Address
                                </button>
                            </form>
                        </div>

                        <div className="bg-white rounded-lg shadow-md p-6">
                            <h2 className="text-xl font-bold mb-4">All Addresses</h2>
                            <div className="space-y-4">
                                {clients.map((client) => (
                                    <div key={client.id}>
                                        <h3 className="font-semibold text-lg mb-2">{client.name}</h3>
                                        {client.directions && client.directions.length > 0 ? (
                                            <div className="ml-4 space-y-2">
                                                {client.directions.map((dir) => (
                                                    <div
                                                        key={dir.id}
                                                        className="border-l-4 border-blue-500 pl-4 py-2"
                                                    >
                                                        📍 {dir.address}
                                                    </div>
                                                ))}
                                            </div>
                                        ) : (
                                            <p className="text-gray-500 ml-4 text-sm">No addresses yet</p>
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