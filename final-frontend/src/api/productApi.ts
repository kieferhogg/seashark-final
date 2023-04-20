import { Product } from "../models/types";
import axios from "axios";

export const getProducts = async (): Promise<any[]> => {
  const response = await axios.get("/api/product");
  return response.data;
}

export const getProduct = async (id: string): Promise<any> => {
  const response = await axios.get(`/api/product/${id}`);
  return response.data;
}

export const createProduct = async (product: Product): Promise<any> => {
  const response = await axios.post("/api/product", product);
  return response.data;
}

export const updateProduct = async (product: Product): Promise<any> => {
  const response = await axios.put(`/api/product/${product.id}`, product);
  return response.data;
}

export const deleteProduct = async (id: number): Promise<void> => {
  await axios.delete(`/api/product/${id}`);
}