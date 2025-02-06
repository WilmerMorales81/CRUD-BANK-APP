import { useState } from "react";
import AccountList from "./AccountList";
import AccountDetails from "./AccountDetails";
import PropTypes from "prop-types";

export default function Accounts({ loggedInUser }) {
  const [detailsAccountId, setDetailsAccountId] = useState(null);

  return (
    <div className="container">
      <div className="row">
        <div className="col-sm-8">
          <AccountList
            setDetailsAccountId={setDetailsAccountId}
            loggedInUser={loggedInUser}
          />
        </div>
        <div className="col-sm-4">
          <AccountDetails detailsAccountId={detailsAccountId} />
        </div>
      </div>
    </div>
  );
}

Accounts.propTypes = {
  loggedInUser: PropTypes.shape({
    roles: PropTypes.arrayOf(PropTypes.string).isRequired,
  }),
};
