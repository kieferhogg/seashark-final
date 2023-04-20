import { Order } from "../models/types";
import axios from "axios";

export const getOrders = async (): Promise<any[]> => {
  const response = await axios.get("/api/order");
  return response.data;
}

export const getOrder = async (id: string): Promise<any> => {
  const response = await axios.get(`/api/order/${id}`);
  return response.data;
}

export const createOrder = async (order: Order): Promise<any> => {
  const response = await axios.post("/api/order", order);
  return response.data;
}

export const updateOrder = async (order: Order): Promise<any> => {
  const response = await axios.put(`/api/order/${order.id}`, order);
  return response.data;
}

export const deleteOrder = async (id: number): Promise<void> => {
  await axios.delete(`/api/order/${id}`);
}