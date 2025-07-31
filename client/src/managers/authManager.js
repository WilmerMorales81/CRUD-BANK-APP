import { getApiUrl } from '../config/api.js';

const _apiUrl = getApiUrl("/api/auth");

// Helper function to handle API responses
const handleResponse = async (response) => {
  const data = await response.json();

  if (!response.ok) {
    console.error("API Error:", data);
    throw new Error(data.message || "An error occurred");
  }

  return data;
};

export const login = async (email, password) => {
  try {
    const response = await fetch(`${_apiUrl}/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ email, password }),
      credentials: "include",
    });

    const data = await handleResponse(response);

    if (data.token) {
      localStorage.setItem("token", data.token);
      return await tryGetLoggedInUser();
    }
    return null;
  } catch (error) {
    console.error("Login error:", error);
    throw error;
  }
};

export const logout = async () => {
  try {
    localStorage.removeItem("token");
    await fetch(`${_apiUrl}/logout`, {
      method: "POST",
      credentials: "include",
    });
  } catch (error) {
    console.error("Logout error:", error);
  } finally {
    window.location.href = "/login";
  }
};

export const tryGetLoggedInUser = async () => {
  const token = localStorage.getItem("token");
  if (!token) return null;

  try {
    const response = await fetch(`${_apiUrl}/me`, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      credentials: "include",
    });

    if (response.status === 401) {
      localStorage.removeItem("token");
      return null;
    }

    return await handleResponse(response);
  } catch (error) {
    console.error("Error fetching user:", error);
    return null;
  }
};

export const register = async (userProfile) => {
  try {
    const response = await fetch(`${_apiUrl}/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(userProfile),
      credentials: "include",
    });

    const data = await handleResponse(response);

    if (data.token) {
      localStorage.setItem("token", data.token);
      return await tryGetLoggedInUser();
    }
    return null;
  } catch (error) {
    console.error("Registration error:", error);
    throw error;
  }
};
