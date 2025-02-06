import {
  Card,
  CardBody,
  CardTitle,
  CardSubtitle,
  Button,
  Spinner,
} from "reactstrap";
import { useState } from "react";
import { payAccount, deleteAccount } from "../../managers/AccountManager";
import PropTypes from "prop-types";

export default function AccountCard({
  account,
  setDetailsAccountId,
  buttons,
  setAccounts,
}) {
  const [selectedOption, setSelectedOption] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [successMessage, setSuccessMessage] = useState("");
  const [accountBalance, setAccountBalance] = useState(account.balance);

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat("en-US", {
      style: "currency",
      currency: "USD",
    }).format(amount);
  };

  const handleSubmitPayment = async () => {
    if (!selectedOption) {
      setError("Please select a payment option");
      return;
    }

    const paymentAmount =
      selectedOption === "balance" ? account.balance : account.minPay;
    const paymentRequest = {
      amount: paymentAmount,
    };

    try {
      setLoading(true);
      setError(null);
      setSuccessMessage("");

      const response = await payAccount(account.id, paymentRequest);

      if (response && response.message) {
        setSuccessMessage(response.message);
        setAccountBalance(response.newBalance);
      } else {
        setError("Failed to process payment");
      }
    } catch (error) {
      setError(error.message || "Failed to process payment");
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (account.balance > 0) {
      window.alert("Cannot delete account with balance");
      return;
    }

    if (!window.confirm("Are you sure you want to delete this account?")) {
      return;
    }

    try {
      setLoading(true);
      setError(null);
      setSuccessMessage("");

      await deleteAccount(account.id);

      setAccounts((prevAccounts) =>
        prevAccounts.filter((acc) => acc.id !== account.id)
      );

      setSuccessMessage("Account deleted successfully");
    } catch (error) {
      console.error("Delete error:", error);
      setError(error.message || "Failed to delete account");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card color="dark" outline style={{ marginBottom: "1rem" }}>
      <CardBody>
        <CardTitle className="mb-4" tag="h5">
          {account.accountType?.name || account.accountTypeName}
        </CardTitle>

        <CardSubtitle className="mb-4 text-muted" tag="h6">
          Account #{account.number}
        </CardSubtitle>

        <CardSubtitle className="mb-2 text-muted" tag="h6">
          Balance: {formatCurrency(account.balance)}
        </CardSubtitle>

        {error && <div className="alert alert-danger mb-3">{error}</div>}

        {successMessage && (
          <div className="alert alert-success mb-3">{successMessage}</div>
        )}

        {buttons ? (
          <div className="d-flex justify-content-end gap-2">
            <Button
              color="dark"
              outline
              onClick={() => {
                setDetailsAccountId(account.id);
                window.scrollTo({ top: 0, behavior: "smooth" });
              }}
            >
              View Details
            </Button>

            <Button color="danger" outline onClick={handleDelete}>
              Delete
            </Button>
          </div>
        ) : (
          <div className="payment-section">
            <div className="payment-options mb-3">
              <div className="form-check mb-2">
                <input
                  className="form-check-input"
                  type="radio"
                  name={`paymentOption-${account.id}`}
                  value="balance"
                  checked={selectedOption === "balance"}
                  onChange={(e) => setSelectedOption(e.target.value)}
                  id={`balance-${account.id}`}
                />
                <label
                  className="form-check-label"
                  htmlFor={`balance-${account.id}`}
                >
                  Full Balance: {formatCurrency(account.balance)}
                </label>
              </div>
              <div className="form-check mb-2">
                <input
                  className="form-check-input"
                  type="radio"
                  name={`paymentOption-${account.id}`}
                  value="minPay"
                  checked={selectedOption === "minPay"}
                  onChange={(e) => setSelectedOption(e.target.value)}
                  id={`minpay-${account.id}`}
                />
                <label
                  className="form-check-label"
                  htmlFor={`minpay-${account.id}`}
                >
                  Minimum Payment: {formatCurrency(account.minPay)}
                </label>
              </div>
            </div>

            <Button
              color="success"
              onClick={handleSubmitPayment}
              disabled={loading || !selectedOption}
              className="w-100 mb-3"
            >
              {loading ? <Spinner size="sm" /> : "Make Payment"}
            </Button>

            <CardSubtitle className="mt-2 text-muted">
              Current Balance: {formatCurrency(accountBalance)}
            </CardSubtitle>
          </div>
        )}
      </CardBody>
    </Card>
  );
}

AccountCard.propTypes = {
  account: PropTypes.shape({
    id: PropTypes.number.isRequired,
    number: PropTypes.string.isRequired,
    balance: PropTypes.number.isRequired,
    minPay: PropTypes.number.isRequired,
    accountTypeName: PropTypes.string,
    accountType: PropTypes.shape({
      name: PropTypes.string,
    }),
  }).isRequired,
  setDetailsAccountId: PropTypes.func.isRequired,
  buttons: PropTypes.bool,
  setAccounts: PropTypes.func.isRequired,
};
