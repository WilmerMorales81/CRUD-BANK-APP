const apiUrl = "/api/accountType";

export const getAccountType = () => {
  return fetch(apiUrl).then((res) => res.json());
};

export const getAccountTypeById = (id) => {
  return fetch(`${apiUrl}/${id}`).then((res) => res.json());
};
