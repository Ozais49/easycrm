import { z } from 'zod'
import type { AddressDto } from '@/lib/api/generated/models'
import { Province } from '@/lib/api/generated/models'

export const PROVINCE_LABELS: Record<NonNullable<Province>, string> = {
  koshi: 'Koshi',
  madhesh: 'Madhesh',
  bagmati: 'Bagmati',
  gandaki: 'Gandaki',
  lumbini: 'Lumbini',
  karnali: 'Karnali',
  sudurpashchim: 'Sudurpashchim',
}

export const PROVINCES = Object.keys(PROVINCE_LABELS) as NonNullable<Province>[]

/** The address inputs of a form; empty string means not set. Spread into a form schema. */
export const addressShape = {
  street: z.string().trim().max(200, 'Use 200 characters or fewer.'),
  city: z.string().trim().max(100, 'Use 100 characters or fewer.'),
  district: z.string().trim().max(100, 'Use 100 characters or fewer.'),
  province: z.union([z.enum(PROVINCES), z.literal('')]),
}

export type AddressFormValues = {
  street: string
  city: string
  district: string
  province: NonNullable<Province> | ''
}

export const orNull = (value: string) => (value === '' ? null : value)

export function toAddressFormValues(address: AddressDto | null | undefined): AddressFormValues {
  return {
    street: address?.street ?? '',
    city: address?.city ?? '',
    district: address?.district ?? '',
    province: address?.province ?? '',
  }
}

export function toAddressDto(values: AddressFormValues): AddressDto {
  return {
    street: orNull(values.street),
    city: orNull(values.city),
    district: orNull(values.district),
    province: values.province === '' ? null : values.province,
  }
}

/** One line for display, or undefined when the address is empty or hidden. */
export function formatAddress(address: AddressDto | null | undefined): string | undefined {
  if (!address) return undefined
  const parts = [
    address.street,
    address.city,
    address.district,
    address.province ? PROVINCE_LABELS[address.province] : null,
  ].filter(Boolean)
  return parts.length > 0 ? parts.join(', ') : undefined
}

/** An optional text field that, when filled, must be a valid email. */
export const optionalEmail = z
  .string()
  .trim()
  .max(254, 'Use 254 characters or fewer.')
  .refine((value) => value === '' || z.email().safeParse(value).success, 'Enter a valid email.')

