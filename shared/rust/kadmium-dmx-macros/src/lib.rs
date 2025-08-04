use proc_macro::TokenStream;
use quote::quote;
use syn::{parse_macro_input, Data, DeriveInput, Fields};

/// Derives implementations of the Fixture trait accessor methods
/// Requires the struct to have fields named: attributes, subscriptions, effects
#[proc_macro_derive(FixtureAccessors)]
pub fn derive_fixture_accessors(input: TokenStream) -> TokenStream {
    let input = parse_macro_input!(input as DeriveInput);
    let name = &input.ident;
    let (impl_generics, ty_generics, where_clause) = input.generics.split_for_impl();

    // Extract field information from the struct
    let fields = match &input.data {
        Data::Struct(data_struct) => match &data_struct.fields {
            Fields::Named(fields) => &fields.named,
            _ => panic!("FixtureAccessors derive macro only supports structs with named fields"),
        },
        _ => panic!("FixtureAccessors derive macro only supports structs"),
    };

    // Find the required fields by name
    let mut attributes_field = None;
    let mut subscriptions_field = None;
    let mut effects_field = None;

    for field in fields {
        let field_name = field.ident.as_ref().unwrap();
        let field_name_str = field_name.to_string();

        if field_name_str == "attributes" {
            attributes_field = Some(field_name);
        } else if field_name_str == "subscriptions" {
            subscriptions_field = Some(field_name);
        } else if field_name_str == "effects" {
            effects_field = Some(field_name);
        }
    }

    let attributes_field = attributes_field.expect("Struct must have an 'attributes' field");
    let subscriptions_field =
        subscriptions_field.expect("Struct must have a 'subscriptions' field");
    let effects_field = effects_field.expect("Struct must have an 'effects' field");

    // Generate the implementation
    let expanded = quote! {
        impl #impl_generics #name #ty_generics #where_clause {
            pub fn fixture_attributes(&self) -> &std::collections::HashMap<String, crate::effects::attribute::Attribute> {
                &self.#attributes_field
            }

            pub fn fixture_subscriptions_mut(&mut self) -> &mut Vec<tokio::sync::broadcast::Receiver<(String, f32)>> {
                &mut self.#subscriptions_field
            }

            pub fn fixture_effects_mut(&mut self) -> &mut Vec<Box<dyn crate::effects::effect::Effect<Self> + Send + Sync>> {
                &mut self.#effects_field
            }

            pub fn fixture_effects(&self) -> &Vec<Box<dyn crate::effects::effect::Effect<Self> + Send + Sync>> {
                &self.#effects_field
            }
        }
    };

    TokenStream::from(expanded)
}
