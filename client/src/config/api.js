// API Configuration
const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5000";

export const getApiUrl = (endpoint) => {
  // If endpoint starts with /, it's a relative path, so we need to add the base URL
  if (endpoint.startsWith('/')) {
    return `${API_BASE_URL}${endpoint}`;
  }
  // If it's already a full URL, return as is
  return endpoint;
};

export default API_BASE_URL; 