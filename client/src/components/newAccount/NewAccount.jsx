import { useState, useEffect } from "react";
import { createAccount } from "../../managers/AccountManager";
import { getAccountType } from "../../managers/AccountTypeManager";
import {
  Container,
  Form,
  FormGroup,
  Label,
  Input,
  Button,
  Alert,
  Row,
  Col,
  Spinner,
} from "reactstrap";

const NewAccount = () => {
  const [accountTypes, setAccountTypes] = useState([]);
  const [selectedAccountType, setSelectedAccountType] = useState("");
  const [initialBalance, setInitialBalance] = useState("");
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const loadAccountTypes = async () => {
      try {
        const data = await getAccountType();
        setAccountTypes(data || []);
      } catch (error) {
        console.error("Error loading account types:", error);
        setErrorMessage("Failed to load account types");
      }
    };

    loadAccountTypes();
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage("");
    setSuccessMessage("");
    setLoading(true);

    try {
      // Validation
      if (!selectedAccountType) {
        throw new Error("Please select an account type");
      }

      const balance = parseFloat(initialBalance);
      if (isNaN(balance) || balance <= 0) {
        throw new Error("Please enter a valid initial balance greater than 0");
      }

      const newAccount = {
        accountTypeId: parseInt(selectedAccountType),
        initialBalance: balance,
      };

      const createdAccount = await createAccount(newAccount);

      setSuccessMessage(
        `Account created successfully! Account Number: ${createdAccount.number}`
      );
      setSelectedAccountType("");
      setInitialBalance("");
    } catch (error) {
      setErrorMessage(
        error.message || "Failed to create account. Please try again."
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container className="mt-5" style={{ maxWidth: "600px" }}>
      <h2 className="mb-4 text-center">Open a New Bank Account</h2>

      {errorMessage && (
        <Alert color="danger" className="mb-4">
          {errorMessage}
        </Alert>
      )}

      {successMessage && (
        <Alert color="success" className="mb-4">
          {successMessage}
        </Alert>
      )}

      <Form onSubmit={handleSubmit} className="bg-light p-4 rounded shadow-sm">
        <FormGroup tag="fieldset">
          <Label className="mb-3">Select Account Type</Label>
          <Row>
            {accountTypes.map((type) => (
              <Col key={type.id} xs="12" sm="6" className="mb-2">
                <FormGroup check>
                  <Label check>
                    <Input
                      type="radio"
                      name="accountType"
                      value={type.id}
                      checked={selectedAccountType === String(type.id)}
                      onChange={(e) => setSelectedAccountType(e.target.value)}
                    />{" "}
                    {type.name}
                  </Label>
                </FormGroup>
              </Col>
            ))}
          </Row>
        </FormGroup>

        <FormGroup>
          <Label for="initialBalance">Initial Deposit Amount</Label>
          <Input
            type="number"
            id="initialBalance"
            placeholder="Enter amount"
            value={initialBalance}
            onChange={(e) => setInitialBalance(e.target.value)}
            min="0"
            step="0.01"
            required
          />
        </FormGroup>

        <Button
          color="primary"
          type="submit"
          block
          disabled={loading}
          className="mt-4"
        >
          {loading ? (
            <>
              <Spinner size="sm" className="me-2" />
              Creating Account...
            </>
          ) : (
            "Open Account"
          )}
        </Button>
      </Form>
    </Container>
  );
};

export default NewAccount;
