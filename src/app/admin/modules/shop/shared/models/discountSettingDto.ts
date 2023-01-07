export interface DiscountSettingDto {
  id?: string;
  name?: string;
  shopId?: string;
  rule?: DiscountMetadataSettingDto,
  action?: DiscountMetadataSettingDto,
  concurrencyToken?: string
}

export interface DiscountMetadataSettingDto {
  id?: string;
  parameters?: any;
}
