import { useState, useEffect } from "react";
import { getPaymentTypes } from "../../managers/PaymentTypeManager";
import { getAccounts } from "../../managers/AccountManager";
import AccountCard from "../Accounts/AccountCard";

export default function PaymentList() {  
  const [filteredAccounts, setFilteredAccounts] = useState([]);
  const [paymentTypes, setPaymentTypes] = useState([]);
  const [selectedPaymentType, setSelectedPaymentType] = useState("");
  const [showAccounts, setShowAccounts] = useState(false);

  const getPaymentTypesList = () => {
    getPaymentTypes().then((data) => {
      setPaymentTypes(data);
    });
  };

  useEffect(() => {
    getPaymentTypesList();
  }, []);

  const handlePaymentTypeChange = async (event) => {
    const selectedType = event.target.value;
    setSelectedPaymentType(selectedType);
    setShowAccounts(false);

    if (selectedType) {
      try {
        const accounts = await getAccounts();
        const filtered = accounts.filter(
          (account) => account.accountTypeId === parseInt(selectedType)
        );
        setFilteredAccounts(filtered);
        setShowAccounts(true);
      } catch (error) {
        console.error("Error loading accounts:", error);
      }
    }
  };

  // Function to update accounts after payment or deletion
  const handleAccountsUpdate = async () => {
    try {
      const accounts = await getAccounts();
      const filtered = accounts.filter(
        (account) => account.accountTypeId === parseInt(selectedPaymentType)
      );
      setFilteredAccounts(filtered);
    } catch (error) {
      console.error("Error refreshing accounts:", error);
    }
  };

  return (
    <div className="payment-list-container">
      <h2 className="mb-4">Payments</h2>
      
      <div className="payment-types-section mb-4">
        <h3>Select Payment Method:</h3>
        <div className="payment-types-grid">
          {paymentTypes.map((type) => (
            <div key={type.id} className="form-check mb-2">
              <input
                type="radio"
                id={`paymentType-${type.id}`}
                name="paymentType"
                value={type.id}
                checked={selectedPaymentType === type.id.toString()}
                onChange={handlePaymentTypeChange}
                className="form-check-input"
              />
              <label 
                className="form-check-label" 
                htmlFor={`paymentType-${type.id}`}
              >
                {type.name}
              </label>
            </div>
          ))}
        </div>
      </div>

      {showAccounts && (
        <div className="accounts-section">
          <h3 className="mb-3">Available Accounts:</h3>
          <div className="accounts-grid">
            {filteredAccounts.length > 0 ? (
              filteredAccounts.map((account) => (
                <AccountCard
                  key={account.id}
                  account={account}
                  setAccounts={handleAccountsUpdate}
                  buttons={false}
                />
              ))
            ) : (
              <p>No accounts available for this payment method.</p>
            )}
          </div>
        </div>
      )}
    </div>
  );
}