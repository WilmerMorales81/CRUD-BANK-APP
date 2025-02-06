import { useState, useEffect } from "react";
import { getPaymentTypes } from "../../managers/PaymentTypeManager";
import { getAccounts } from "../../managers/AccountManager";
import AccountCard from "../Accounts/AccountCard";

// eslint-disable-next-line react/prop-types
export default function PaymentList({ setDetailsAccountId }) {
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

  const handlePaymentTypeChange = (event) => {
    const selectedType = event.target.value;
    setSelectedPaymentType(selectedType);
    setShowAccounts(false); // Ocultar las cuentas inicialmente

    if (selectedType) {
      // Cargar cuentas solo para el tipo de pago seleccionado
      getAccounts().then((data) => {
        const filtered = data.filter(
          (account) => account.accountTypeId === parseInt(selectedType)
        );
        setFilteredAccounts(filtered);
        setShowAccounts(true); // Mostrar las cuentas después de cargar
      });
    }
  };

  return (
    <>
      <h2>Payments</h2>
      <div>
        <h3>Select Payment Method:</h3>
        {paymentTypes.map((type) => (
          <div key={type.id}>
            <input
              type="radio"
              id={`paymentType-${type.id}`}
              name="paymentType"
              value={type.id}
              checked={selectedPaymentType === type.id.toString()}
              onChange={handlePaymentTypeChange}
            />
            <label htmlFor={`paymentType-${type.id}`}>{type.name}</label>
          </div>
        ))}
      </div>

      {showAccounts && (
        <>
          <h3>Accounts you can pay with this method:</h3>
          <ul>
            {filteredAccounts.map((account) => (
              <AccountCard
                account={account}
                setDetailsAccountId={setDetailsAccountId}
                key={account.id}
                buttons={false}
              />
            ))}
          </ul>
        </>
      )}
    </>
  );
}
