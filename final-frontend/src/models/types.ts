export type Product = {
  id: number;
  name: string;
  description: string;
};

export type Order = {
  id: number;
  quantity: number;
  productID: number;
};