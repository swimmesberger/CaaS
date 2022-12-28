import { KeysetPaginationDirection } from './keysetPaginationDirection';


export interface ParsedPaginationToken {
    direction?: KeysetPaginationDirection;
    reference?: string | null;
}

