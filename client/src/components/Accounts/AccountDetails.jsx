import { useState, useEffect } from "react";
import {
  Card,
  CardTitle,
  CardBody,
  CardText,
  Button,
  Spinner,
} from "reactstrap";
import { getCustomerByAccountId } from "../../managers/userProfileManager";
import { useNavigate } from "react-router-dom";
import PropTypes from "prop-types";

export default function AccountDetails({ detailsAccountId }) {
  const [customer, setCustomer] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const navigate = useNavigate();

  useEffect(() => {
    const fetchDetails = async () => {
      if (!detailsAccountId) return;

      setLoading(true);
      setError(null);
      try {
        const customerData = await getCustomerByAccountId(detailsAccountId);
        setCustomer(customerData);
      } catch (err) {
        setError(err.message || "Failed to load customer details");
        console.error("Error fetching customer:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchDetails();
  }, [detailsAccountId]);

  if (loading) {
    return (
      <div className="d-flex justify-content-center p-5">
        <Spinner color="primary" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="alert alert-danger m-3">
        <h4>Error Loading Customer Details</h4>
        <p>{error}</p>
      </div>
    );
  }

  if (!detailsAccountId) {
    return (
      <div className="text-center p-4">
        <h2>Customer Details</h2>
        <p className="text-muted">Select an account to view customer details</p>
      </div>
    );
  }

  return (
    <div className="customer-details m-3">
      <h2>Customer Information</h2>
      <Card>
        <CardBody>
          {customer ? (
            <>
              <CardTitle tag="h5">
                {customer.firstName} {customer.lastName}
              </CardTitle>
              <CardText>
                <strong>Address:</strong> {customer.address}
              </CardText>
              <CardText>
                <strong>Phone:</strong> {customer.phone}
              </CardText>
              <CardText>
                <strong>Email:</strong> {customer.email}
              </CardText>
              <div className="text-end mt-3">
                <Button
                  color="success"
                  outline
                  onClick={() => navigate(`/customers/${detailsAccountId}`)}
                >
                  Edit Info
                </Button>
              </div>
            </>
          ) : (
            <p>No customer information available</p>
          )}
        </CardBody>
      </Card>
    </div>
  );
}

AccountDetails.propTypes = {
  detailsAccountId: PropTypes.number,
};
