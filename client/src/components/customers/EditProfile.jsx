import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  Button,
  Form,
  FormGroup,
  Label,
  Input,
  Spinner,
} from "reactstrap";
import {
  getCustomerByAccountId,
  updateUserProfile,
} from "../../managers/userProfileManager";
import PropTypes from "prop-types"; // Add PropTypes


export default function EditProfile({ loggedInUser }) {
  const { id } = useParams();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    address: "",
    phone: "",
    email: "",
  });

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    const fetchUserProfile = async () => {
      try {
        const userData = await getCustomerByAccountId(id);
        setFormData({
          firstName: userData.firstName || "",
          lastName: userData.lastName || "",
          address: userData.address || "",
          phone: userData.phone || "",
          email: userData.email || "",
        });
      } catch (err) {
        setError("Failed to load user profile");
        console.error("Error fetching profile:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchUserProfile();
  }, [id]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    setError(null);
    setSuccess(false);
  };

  const validateForm = () => {
    const errors = [];

    if (!formData.firstName.trim()) errors.push("First name is required");
    if (!formData.lastName.trim()) errors.push("Last name is required");
    if (!formData.address.trim()) errors.push("Address is required");

    // Phone validation (US format)
    const phoneRegex = /^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$/;
    if (formData.phone && !phoneRegex.test(formData.phone)) {
      errors.push("Please enter a valid phone number (e.g., 123-456-7890)");
    }

    return errors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const errors = validateForm();
    if (errors.length > 0) {
      setError(errors.join(", "));
      return;
    }

    setSaving(true);
    setError(null);
    setSuccess(false);

    try {
      // Only send updatable fields
      const updateData = {
        firstName: formData.firstName,
        lastName: formData.lastName,
        address: formData.address,
        phone: formData.phone,
      };

      await updateUserProfile(id, updateData);
      setSuccess(true);
      const successMessage = loggedInUser?.roles?.includes("Admin") 
        ? "Profile updated successfully! Redirecting to accounts..."
        : "Your profile has been updated! Redirecting...";
      
      setSuccess(successMessage);
      setTimeout(() => navigate("/accounts"), 1500);
    } catch (err) {
      setError(err.message || "Failed to update profile information");
      console.error("Error updating profile:", err);
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="text-center p-4">
        <Spinner color="primary" />
      </div>
    );
  }

  return (
    <div className="container py-4">
      <h2 className="mb-4">
        {loggedInUser?.roles?.includes("Admin") 
          ? "Edit Customer Profile" 
          : "Edit Profile Information"}
      </h2>

      {error && (
        <div className="alert alert-danger mb-4">
          {error}
        </div>
      )}

      {success && (
        <div className="alert alert-success mb-4">
          Profile updated successfully! Redirecting...
        </div>
      )}

      <Form onSubmit={handleSubmit} className="bg-light p-4 rounded shadow-sm">
        <div className="row">
          <div className="col-md-6">
            <FormGroup>
              <Label for="firstName">First Name *</Label>
              <Input
                type="text"
                name="firstName"
                id="firstName"
                value={formData.firstName}
                onChange={handleChange}
                placeholder="Enter first name"
                required
              />
            </FormGroup>
          </div>

          <div className="col-md-6">
            <FormGroup>
              <Label for="lastName">Last Name *</Label>
              <Input
                type="text"
                name="lastName"
                id="lastName"
                value={formData.lastName}
                onChange={handleChange}
                placeholder="Enter last name"
                required
              />
            </FormGroup>
          </div>
        </div>

        <FormGroup>
          <Label for="address">Address *</Label>
          <Input
            type="text"
            name="address"
            id="address"
            value={formData.address}
            onChange={handleChange}
            placeholder="Enter full address"
            required
          />
        </FormGroup>

        <FormGroup>
          <Label for="phone">Phone Number *</Label>
          <Input
            type="tel"
            name="phone"
            id="phone"
            value={formData.phone}
            onChange={handleChange}
            placeholder="123-456-7890"
            required
          />
        </FormGroup>

        <FormGroup>
          <Label for="email">Email Address</Label>
          <Input
            type="email"
            name="email"
            id="email"
            value={formData.email}
            disabled
            className="bg-light"
          />
          <small className="text-muted">
            Email cannot be changed. Please contact support if you need to
            update your email.
          </small>
        </FormGroup>

        <div className="d-flex gap-2 mt-4">
          <Button type="submit" color="primary" disabled={saving}>
            {saving ? <Spinner size="sm" /> : "Save Changes"}
          </Button>

          <Button
            type="button"
            color="secondary"
            outline
            onClick={() => navigate("/accounts")}
            disabled={saving}
          >
            Cancel
          </Button>
        </div>
      </Form>
    </div>
  );
}


EditProfile.propTypes = {
  loggedInUser: PropTypes.shape({
    roles: PropTypes.arrayOf(PropTypes.string),
    id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  }),
};