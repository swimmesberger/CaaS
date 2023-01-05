import {ShopAdminDto} from "./shopAdminDto";

export interface ShopDto {
  id?: string;
  name?: string | null;
  appKey?: string;
  shopAdmin?: ShopAdminDto;
}

