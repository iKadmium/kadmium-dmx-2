/// A derive macro for generating accessor methods for the Fixture trait.
/// This macro generates the four accessor methods that are commonly implemented
/// for structs that implement the Fixture trait.
///
/// The macro expects the struct to have fields named:
/// - `attributes: HashMap<String, Attribute>`
/// - `subscriptions: HashMap<String, broadcast::Receiver<f32>>`
/// - `effects: Vec<Box<dyn Effect<Self> + Send + Sync>>`
///
/// # Example
///
/// ```rust
/// #[derive(Debug, FixtureAccessors)]
/// pub struct MyFixture {
///     pub attributes: HashMap<String, Attribute>,
///     subscriptions: HashMap<String, broadcast::Receiver<f32>>,
///     pub effects: Vec<Box<dyn Effect<MyFixture> + Send + Sync>>,
/// }
/// ```
macro_rules! fixture_accessors {
    ($struct_name:ident) => {
        use crate::fixtures::fixture::FixtureAccessors;

        impl FixtureAccessors for $struct_name {
            /// Returns a reference to the attributes HashMap
            fn attributes(&self) -> &std::collections::HashMap<String, crate::effects::attribute::Attribute> {
                &self.attributes
            }

            /// Returns a mutable reference to the subscriptions HashMap
            fn subscriptions_mut(&mut self) -> &mut std::collections::HashMap<String, tokio::sync::broadcast::Receiver<f32>> {
                &mut self.subscriptions
            }

            /// Returns a mutable reference to the effects Vec
            fn effects_mut(&mut self) -> &mut Vec<Box<dyn crate::effects::effect::Effect<Self> + Send + Sync>> {
                &mut self.effects
            }

            /// Returns a reference to the effects Vec
            fn effects(&self) -> &Vec<Box<dyn crate::effects::effect::Effect<Self> + Send + Sync>> {
                &self.effects
            }

            /// Updates the subscriptions HashMap with new receivers
            fn update_subscriptions(&mut self, attribute_receivers: std::collections::HashMap<String, tokio::sync::broadcast::Receiver<f32>>) {
                self.subscriptions = attribute_receivers;
            }
        }
    };
}

pub(crate) use fixture_accessors;
