const apiUrl = "/api/paymentType";

export const getPaymentTypes = () => {
  return fetch(apiUrl).then((res) => res.json());
};

export const getPaymentTypeById = (id) => {
  return fetch(`${apiUrl}/${id}`).then((res) => res.json());
};
