import { useState } from "react";
import { register } from "../../managers/authManager";
import { Link, useNavigate } from "react-router-dom";
import {
  Button,
  FormFeedback,
  FormGroup,
  Input,
  Label,
  Alert,
} from "reactstrap";
import PropTypes from "prop-types";

export default function Register({ setLoggedInUser }) {
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    address: "",
    password: "",
    confirmPassword: "",
  });
  const [errors, setErrors] = useState({});
  const [loading, setLoading] = useState(false);
  const [apiError, setApiError] = useState("");

  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    if (errors[name]) {
      setErrors((prev) => ({ ...prev, [name]: "" }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.firstName.trim())
      newErrors.firstName = "First name is required";
    if (!formData.lastName.trim()) newErrors.lastName = "Last name is required";
    if (!formData.email.trim()) newErrors.email = "Email is required";
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = "Invalid email format";
    }
    if (!formData.phone.trim()) newErrors.phone = "Phone is required";
    if (!/^\d{10}$/.test(formData.phone.replace(/\D/g, ""))) {
      newErrors.phone = "Phone must be 10 digits";
    }
    if (!formData.address.trim()) newErrors.address = "Address is required";
    if (formData.password.length < 8) {
      newErrors.password = "Password must be at least 8 characters";
    }
    if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.password)) {
      newErrors.password =
        "Password must contain uppercase, lowercase, and numbers";
    }
    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = "Passwords do not match";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    setLoading(true);
    setApiError("");

    try {
      const newUser = {
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        phone: formData.phone.replace(/\D/g, ""), // Remove non-digits
        address: formData.address,
        password: formData.password,
      };

      const user = await register(newUser);
      if (user) {
        setLoggedInUser(user);
        navigate("/");
      } else {
        setApiError("Registration failed. Please try again.");
      }
    } catch (error) {
      console.error("Registration error:", error);
      setApiError(error.message || "An error occurred during registration");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-5" style={{ maxWidth: "500px" }}>
      <h2 className="text-center mb-4">Registration Form</h2>
      {apiError && <Alert color="danger">{apiError}</Alert>}

      <form onSubmit={handleSubmit} className="bg-light p-4 rounded shadow-sm">
        <FormGroup>
          <Label for="firstName">First Name</Label>
          <Input
            id="firstName"
            name="firstName"
            value={formData.firstName}
            onChange={handleChange}
            invalid={!!errors.firstName}
            placeholder="Enter your first name"
          />
          {errors.firstName && <FormFeedback>{errors.firstName}</FormFeedback>}
        </FormGroup>

        <FormGroup>
          <Label for="lastName">Last Name</Label>
          <Input
            id="lastName"
            name="lastName"
            value={formData.lastName}
            onChange={handleChange}
            invalid={!!errors.lastName}
            placeholder="Enter your last name"
          />
          {errors.lastName && <FormFeedback>{errors.lastName}</FormFeedback>}
        </FormGroup>

        <FormGroup>
          <Label for="email">Email</Label>
          <Input
            id="email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            invalid={!!errors.email}
            placeholder="example@email.com"
          />
          {errors.email && <FormFeedback>{errors.email}</FormFeedback>}
        </FormGroup>

        <FormGroup>
          <Label for="phone">Phone Number</Label>
          <Input
            id="phone"
            name="phone"
            value={formData.phone}
            onChange={handleChange}
            invalid={!!errors.phone}
            placeholder="(123) 456-7890"
          />
          {errors.phone && <FormFeedback>{errors.phone}</FormFeedback>}
        </FormGroup>

        <FormGroup>
          <Label for="address">Address</Label>
          <Input
            id="address"
            name="address"
            value={formData.address}
            onChange={handleChange}
            invalid={!!errors.address}
            placeholder="Enter your full address"
          />
          {errors.address && <FormFeedback>{errors.address}</FormFeedback>}
        </FormGroup>

        <FormGroup>
          <Label for="password">Password</Label>
          <Input
            id="password"
            name="password"
            type="password"
            value={formData.password}
            onChange={handleChange}
            invalid={!!errors.password}
            placeholder="Create a strong password"
          />
          {errors.password && <FormFeedback>{errors.password}</FormFeedback>}
        </FormGroup>

        <FormGroup>
          <Label for="confirmPassword">Confirm Password</Label>
          <Input
            id="confirmPassword"
            name="confirmPassword"
            type="password"
            value={formData.confirmPassword}
            onChange={handleChange}
            invalid={!!errors.confirmPassword}
            placeholder="Confirm your password"
          />
          {errors.confirmPassword && (
            <FormFeedback>{errors.confirmPassword}</FormFeedback>
          )}
        </FormGroup>

        <Button
          color="primary"
          type="submit"
          disabled={loading}
          className="w-100 mt-3"
        >
          {loading ? (
            <>
              <span
                className="spinner-border spinner-border-sm me-2"
                role="status"
                aria-hidden="true"
              ></span>
              Creating Account...
            </>
          ) : (
            "Create Account"
          )}
        </Button>
      </form>

      <p className="text-center mt-3">
        Already have an account? <Link to="/login">Sign in here</Link>
      </p>
    </div>
  );
}

Register.propTypes = {
  setLoggedInUser: PropTypes.func.isRequired,
};
