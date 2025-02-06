import { useState, useEffect } from "react";
import { getAccounts, getAccountsByUser } from "../../managers/AccountManager";
import { getAccountType } from "../../managers/AccountTypeManager";
import { Spinner, Alert } from "reactstrap";
import { useNavigate } from "react-router-dom";
import AccountCard from "./AccountCard";
import PropTypes from "prop-types";

export default function AccountList({ setDetailsAccountId, loggedInUser }) {
  const [accounts, setAccounts] = useState([]);
  const [filteredAccounts, setFilteredAccounts] = useState([]);
  const [accountTypes, setAccountTypes] = useState([]);
  const [selectedAccountType, setSelectedAccountType] = useState("all");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      if (!loggedInUser) {
        navigate("/login");
        return;
      }

      try {
        setLoading(true);
        setError(null);

        let accountsData;
        if (loggedInUser.roles?.includes("Admin")) {
          accountsData = await getAccounts();
        } else {
          accountsData = await getAccountsByUser();
        }

        const typesData = await getAccountType();

        setAccounts(accountsData || []);
        setFilteredAccounts(accountsData || []);
        setAccountTypes(typesData || []);
      } catch (err) {
        console.error("Error fetching data:", err);
        setError("Failed to load accounts");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [loggedInUser, navigate]);

  const handleAccountTypeChange = (event) => {
    const selectedType = event.target.value;
    setSelectedAccountType(selectedType);

    if (selectedType === "all") {
      setFilteredAccounts(accounts);
    } else {
      setFilteredAccounts(
        accounts.filter(
          (account) => account.accountTypeId === parseInt(selectedType, 10)
        )
      );
    }
  };

  if (loading) {
    return (
      <div className="text-center p-4">
        <Spinner color="primary" />
      </div>
    );
  }

  if (error) {
    return <Alert color="danger">{error}</Alert>;
  }

  return (
    <div>
      <h2>Accounts</h2>

      {loading && <div>Loading...</div>}
      {error && <div className="alert alert-danger">{error}</div>}

      {!loading && !error && (
        <>
          <div className="mb-3">
            <label htmlFor="accountTypeSelect">Select Account Type:</label>
            <select
              id="accountTypeSelect"
              value={selectedAccountType}
              onChange={handleAccountTypeChange}
              className="form-select"
            >
              <option value="all">Show All</option>
              {accountTypes.map((type) => (
                <option key={type.id} value={type.id}>
                  {type.name}
                </option>
              ))}
            </select>
          </div>

          {filteredAccounts.length > 0 ? (
            filteredAccounts.map((account) => (
              <AccountCard
                key={account.id}
                account={account}
                setDetailsAccountId={setDetailsAccountId}
                buttons={true}
                setAccounts={setAccounts}
              />
            ))
          ) : (
            <p>No accounts found.</p>
          )}
        </>
      )}
    </div>
  );
}

AccountList.propTypes = {
  setDetailsAccountId: PropTypes.func.isRequired,
  loggedInUser: PropTypes.shape({
    roles: PropTypes.arrayOf(PropTypes.string),
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  }),
};
