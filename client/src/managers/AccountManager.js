const apiUrl = "/api/accounts";

// Obtener todas las cuentas
export const getAccounts = async () => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${apiUrl}`, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching accounts:", error);
    throw error;
  }
};

// Obtener una cuenta por ID
export const getAccountById = (id) => {
  return fetch(`${apiUrl}/${id}`)
    .then((res) => res.json())
    .catch((error) => {
      console.error("Error fetching account:", error);
    });
};

export const payAccount = async (accountId, paymentData) => {
  try {
    if (paymentData.amount <= 0) {
      throw new Error("Payment amount must be greater than zero");
    }

    const token = localStorage.getItem("token");
    // Fix the URL to include /api
    const response = await fetch(`/api/accounts/pay/${accountId}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(paymentData),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => null);
      throw new Error(
        errorData?.message || `Payment failed (${response.status})`
      );
    }

    return response.json();
  } catch (error) {
    console.error("Error processing payment:", error);
    throw error;
  }
};

export const deleteAccount = async (accountId) => {
  if (!accountId) {
    throw new Error("Account ID is required");
  }

  const token = localStorage.getItem("token");
  if (!token) {
    throw new Error("Authentication required");
  }

  try {
    const response = await fetch(`${apiUrl}/${accountId}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      // Try to get error message from response
      const errorData = await response.json().catch(() => null);
      if (response.status === 400) {
        throw new Error(
          errorData?.message || "Cannot delete account with balance"
        );
      }
      throw new Error(
        errorData?.message || `Failed to delete account (${response.status})`
      );
    }

    return true;
  } catch (error) {
    console.error("Delete account error:", error);
    throw error;
  }
};

// Crear una nueva cuenta
export const createAccount = async (createAccountRequest) => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${apiUrl}`, {
      // Use the base apiUrl
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(createAccountRequest),
    });

    // Check if response is ok before trying to parse JSON
    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || "Failed to create account");
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error creating account:", error);
    throw error;
  }
};

export const getAccountsByUser = async () => {
  try {
    const token = localStorage.getItem("token");
    const response = await fetch(`${apiUrl}/user`, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    return data;
  } catch (error) {
    console.error("Error fetching user accounts:", error);
    throw error;
  }
};
