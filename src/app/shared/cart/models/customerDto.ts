export interface CustomerDto {
    id?: string;
    firstName?: string | null;
    lastName?: string | null;
    shopId?: string;
    eMail?: string | null;
    telephoneNumber?: string | null;
    creditCardNumber: string | null;
    concurrencyToken?: string | null;
}

