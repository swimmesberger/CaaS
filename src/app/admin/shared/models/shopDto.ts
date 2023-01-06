import {ShopAdminDto} from "./shopAdminDto";

export interface ShopDto {
  id?: string;
  name?: string;
  appKey?: string;
  shopAdmin?: ShopAdminDto;
  cartLifetimeMinutes?: number,
  concurrencyToken?: string
}

