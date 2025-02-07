const apiUrl = "/api/accounts";

const getAuthHeaders = () => ({
  "Content-Type": "application/json",
  Authorization: `Bearer ${localStorage.getItem("token")}`,
});

// Helper function for API calls
const handleResponse = async (response) => {
  if (!response.ok) {
    if (response.status === 403) {
      throw new Error("You don't have permission to access this resource");
    }
    const errorData = await response.json().catch(() => null);
    throw new Error(errorData?.message || `API Error: ${response.status}`);
  }
  return response.json();
};

// Get user profile by account ID
export const getCustomerByAccountId = async (accountId) => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${apiUrl}/${accountId}/customer`, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => null);
      throw new Error(
        errorData?.message || `Failed to fetch profile (${response.status})`
      );
    }

    return await response.json();
  } catch (error) {
    console.error("Error fetching user profile:", error);
    throw error;
  }
};

// Get current user profile
export const getCurrentUser = async () => {
  try {
    const response = await fetch(`${apiUrl}/me`, {
      headers: getAuthHeaders(),
    });
    return handleResponse(response);
  } catch (error) {
    console.error("Error fetching current user:", error);
    throw error;
  }
};

// Update user profile
export const updateUserProfile = async (accountId, profileData) => {
  try {
    const token = localStorage.getItem("token");
    // Update URL to match the backend endpoint
    const response = await fetch(`/api/accounts/${accountId}/customer`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(profileData),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => null);
      throw new Error(
        errorData?.message || `Failed to update profile (${response.status})`
      );
    }

    return await response.json();
  } catch (error) {
    console.error("Error updating user profile:", error);
    throw error;
  }
};
