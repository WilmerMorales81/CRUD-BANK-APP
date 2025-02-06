import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { login } from "../../managers/authManager";
import {
  Button,
  FormFeedback,
  FormGroup,
  Input,
  Label,
  Spinner,
  Alert,
} from "reactstrap";
import PropTypes from "prop-types";

export default function Login({ setLoggedInUser }) {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });
  const [failedLogin, setFailedLogin] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    // Clear errors when user types
    setFailedLogin(false);
    setErrorMessage("");
  };

  const validateEmail = (email) => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setFailedLogin(false);
    setErrorMessage("");

    // Validate email format
    if (!validateEmail(formData.email)) {
      setErrorMessage("Please enter a valid email address");
      setLoading(false);
      return;
    }

    try {
      const user = await login(formData.email, formData.password);
      if (!user) {
        setFailedLogin(true);
        setErrorMessage("Invalid email or password");
      } else {
        console.log("Login successful:", user);
        setLoggedInUser(user);
        navigate("/");
      }
    } catch (error) {
      console.error("Error during login:", error);
      setFailedLogin(true);
      setErrorMessage(error.message || "An error occurred during login");
    } finally {
      setLoading(false);
    }
  };

  // Validate if form is complete
  const isFormValid =
    formData.email.trim() !== "" && formData.password.trim() !== "";

  return (
    <div className="container" style={{ maxWidth: "500px", marginTop: "2rem" }}>
      <div className="text-center mb-4">
        <h3 className="mb-3">Welcome to</h3>
        <img
          src="./CrudBankLogo.png"
          alt="CrudBank"
          style={{ maxWidth: "200px", marginBottom: "2rem" }}
        />
        <h3>Login</h3>
      </div>

      {errorMessage && (
        <Alert color="danger" className="mb-4">
          {errorMessage}
        </Alert>
      )}

      <form onSubmit={handleSubmit}>
        <FormGroup>
          <Label for="email">Email</Label>
          <Input
            id="email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            invalid={failedLogin}
            placeholder="Enter your email"
            autoComplete="email"
          />
          {failedLogin && <FormFeedback>Please check your email</FormFeedback>}
        </FormGroup>

        <FormGroup>
          <Label for="password">Password</Label>
          <Input
            id="password"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleChange}
            invalid={failedLogin}
            placeholder="Enter your password"
            autoComplete="current-password"
          />
          {failedLogin && (
            <FormFeedback>Please check your password</FormFeedback>
          )}
        </FormGroup>

        <Button
          color="success"
          type="submit"
          className="w-100 mb-3"
          disabled={!isFormValid || loading}
        >
          {loading ? (
            <>
              <Spinner size="sm" className="me-2" />
              Logging in...
            </>
          ) : (
            "Login"
          )}
        </Button>
      </form>

      <div className="text-center mt-3">
        <p className="mb-0">
          Not signed up yet?{" "}
          <Link to="/register" className="text-success">
            Register here
          </Link>
        </p>
      </div>
    </div>
  );
}

Login.propTypes = {
  setLoggedInUser: PropTypes.func.isRequired,
};
